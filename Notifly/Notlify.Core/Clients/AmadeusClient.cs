using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notlify.Core00.Interfaces;
using Notlify.Core00.Models.Amadeus;

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
    
    public async Task<decimal?> GetFlightPriceAsync(string origin, string destination, string departureDate)
    {
        try
        {
            await EnsureAuthenticated();
            var url = $"https://test.api.amadeus.com/v2/shopping/flight-offers?originLocationCode={origin}&destinationLocationCode={destination}&departureDate={departureDate}&adults=1&currencyCode=GBP&max=1";

            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Amadeus API Error ({response.StatusCode})");
                return null;
            }
            
            var content = await response.Content.ReadFromJsonAsync<FlightOffersResponse>();
            
            if (content == null)
            {
                _logger.LogError("Amadeus API returned null content.");
                return null;
            }
            
            if (content.Data.Count > 0)
                return content.Data.First().Price.GrandTotalAmount;

            _logger.LogWarning($"No flights found for {origin}->{destination} on {departureDate}");
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError("Error fetching flight price: {Message}", e.Message);
            return null;
        }
    }

    private async Task EnsureAuthenticated()
    {
        try
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cachedToken);
                return;
            }
            
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
            
            var res = await _httpClient.SendAsync(request);

            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new Exception($"Failed to authenticate with Amadeus: {errorContent}");
            }

            var content = await res.Content.ReadFromJsonAsync<AuthenticationResponse>();
            
            
            _cachedToken = content?.AccessToken;
            int expiresInSeconds = (int)content!.ExpiresIn;
            
            _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresInSeconds);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cachedToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Authentication failed: {Message}", e.Message);
        }
    }
}