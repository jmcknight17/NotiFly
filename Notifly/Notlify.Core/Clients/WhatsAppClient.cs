using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notlify.Core00.Interfaces;

namespace Notlify.Core00;

public class WhatsAppClient : IWhatsAppClient
{
    private readonly ILogger<WhatsAppClient> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public WhatsAppClient(ILogger<WhatsAppClient> logger, HttpClient httpClient, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
        
        var authToken = _configuration["WhatsApp:AuthToken"];
        if (!string.IsNullOrEmpty(authToken))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
    }

    public Task<bool> SendPriceAlertAsync(string msgBody)
    {
        try
        {
            var phoneNumberId = _configuration["WhatsApp:PhoneNumberId"];
            var recipientNumber = _configuration["WhatsApp:RecipientNumber"];
        
            if (string.IsNullOrEmpty(phoneNumberId) || string.IsNullOrEmpty(recipientNumber))
            {
                _logger.LogError("WhatsApp configuration is missing.");
                return Task.FromResult(false);
            }
            
            var requestUri = $"https://graph.facebook.com/v22.0/{phoneNumberId}/messages";
            var requestBody = new
            {
                messaging_product = "whatsapp",
                to = recipientNumber,
                type = "text",
                text = new
                {
                    body = msgBody
                }
            };
            
            var res = _httpClient.PostAsJsonAsync(requestUri, requestBody);
            if (!res.Result.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send WhatsApp message. Status Code: {StatusCode}", res.Result.StatusCode);
                return Task.FromResult(false);
            }
            
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            return Task.FromResult(false);
        }
    }
}