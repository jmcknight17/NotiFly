using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notlify.Core00.Interfaces;

namespace Notlify.Core00;

public class AmadeusClient : IAmadeusClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configurationProvider;
    private readonly ILogger<AmadeusClient> _logger;
    
    private static string? _cachedToken;
    private static DateTime _tokenExpiry;
    
    public AmadeusClient(HttpClient httpClient, IConfiguration configurationProvider, ILogger<AmadeusClient> logger)
    {
        _httpClient = httpClient;
        _configurationProvider = configurationProvider;
        _logger = logger;
    }
    
    public async Task<decimal?> GetFlightPriceAsync(string origin, string destination, DateTime departureDate)
    {
        try
        {
            await EnsureAuthenticated();
            var url = $"https://test.api.amadeus.com/v2/shopping/flight-offers?originLocationCode={origin}&destinationLocationCode={destination}&departureDate={departureDate}&adults=1&currencyCode=GBP&max=1";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Amadeus API Error ({response.StatusCode}): {content}");
                return null;
            }

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            // Check if "data" array exists and has items
            if (root.TryGetProperty("data", out var data) && data.GetArrayLength() > 0)
            {
                // Navigate JSON: data[0] -> price -> grandTotal
                var priceString = data[0].GetProperty("price").GetProperty("grandTotal").GetString();
                    
                if (decimal.TryParse(priceString, out decimal price))
                {
                    return price;
                }
            }

            _logger.LogWarning($"No flights found for {origin}->{destination} on {departureDate}");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task EnsureAuthenticated()
    {
        try
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachedToken);
                return;
            }
            _logger.LogInformation("Notifly client not authenticated");
            
            var clientKey = _configurationProvider["Amadeus:ClientKey"];
            var clientSecret = _configurationProvider["Amadeus:ClientSecret"];

            var request = new HttpRequestMessage(HttpMethod.Post, "https://test.api.amadeus.com/v1/security/oauth2/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", clientKey },
                    { "client_secret", clientSecret }
                })
            };
            
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to authenticate with Amadeus: {content}");
            }
            
            using var doc = JsonDocument.Parse(content);
            _cachedToken = doc.RootElement.GetProperty("access_token").GetString();
            int expiresInSeconds = doc.RootElement.GetProperty("expires_in").GetInt32();
            
            _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresInSeconds);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cachedToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}