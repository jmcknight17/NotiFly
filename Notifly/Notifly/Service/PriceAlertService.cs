using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Notlify.Core00.Interfaces;

namespace Notifly.Service;

public class PriceAlertService
{
    private readonly ILogger<PriceAlertService> _logger;
    private readonly IAmadeusClient _amadeusClient;
    private readonly IWhatsAppClient _whatsAppClient;

    public PriceAlertService(ILogger<PriceAlertService> logger, IAmadeusClient amadeusClient,
        IWhatsAppClient whatsAppClient)
    {
        _logger = logger;
        _amadeusClient = amadeusClient;
        _whatsAppClient = whatsAppClient;
    }

    private const decimal PriceThreshold = 200m;
    private readonly DateTime _departureDate = new DateTime(2024, 5, 27);
    private const string Origin = "LHR";
    private const string Destination = "JFK";

    [Function("PriceAlertService")]
    public async Task CheckAndSendPriceAlertAsync([TimerTrigger("0 12 * * *")] TimerInfo myTimer,
        FunctionContext context)
    {
        try
        {
            var price = await _amadeusClient.GetFlightPriceAsync(Origin, Destination, _departureDate);
            if (price == null)
                throw new Exception($"Price not found for {_departureDate}");

            if (price.Value < PriceThreshold)
            {
                var msgBody =
                    $"Price Alert! Flight from {Origin} to {Destination} on {_departureDate:yyyy-MM-dd} is now ${price.Value}.";
                var success = await _whatsAppClient.SendPriceAlertAsync(msgBody);
                if (success)
                {
                    _logger.LogInformation("Price alert sent successfully.");
                }
                else
                {
                    _logger.LogWarning("Failed to send price alert.");
                }
            }
            else
            {
                _logger.LogInformation("No price alert needed. Current price: {Price}",
                    price.HasValue ? price.Value.ToString() : "N/A");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}