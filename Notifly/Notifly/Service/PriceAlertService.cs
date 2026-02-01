using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notifly.Builders;
using Notifly.Mappers;
using Notifly.Service.Models;
using Notlify.Core00.Interfaces;

namespace Notifly.Service;

public class PriceAlertService
{
    private readonly ILogger<PriceAlertService> _logger;
    private readonly IAmadeusClient _amadeusClient;
    private readonly IWhatsAppClient _whatsAppClient;
    private readonly IFlightInfoMapper _flightInfoMapper;
    private readonly IConfiguration _configuration;
    private readonly IWhatsAppNotificationBuilder _whatsAppNotificationBuilder;

    public PriceAlertService(ILogger<PriceAlertService> logger, IAmadeusClient amadeusClient,
        IWhatsAppClient whatsAppClient, IFlightInfoMapper flightInfoMapper, IConfiguration configuration, IWhatsAppNotificationBuilder whatsAppNotificationBuilder)
    {
        _logger = logger;
        _amadeusClient = amadeusClient;
        _whatsAppClient = whatsAppClient;
        _flightInfoMapper = flightInfoMapper;
        _configuration = configuration;
        _whatsAppNotificationBuilder = whatsAppNotificationBuilder;
    }
    
    [Function("PriceAlertService")]
    public async Task CheckAndSendPriceAlertAsync([TimerTrigger("0 12 * * *")] TimerInfo myTimer,
        FunctionContext context)
    {
        try
        {
            var priceThresholdRaw = _configuration["PriceThreshold"];
            var flightDateRaw = _configuration["FlightDate"];
            var rawPaths = _configuration["FlightPaths"];
            if(string.IsNullOrWhiteSpace(rawPaths) || string.IsNullOrWhiteSpace(flightDateRaw) || string.IsNullOrWhiteSpace(priceThresholdRaw))
                throw new Exception("Failed to load configuration.");
            
            decimal.TryParse(priceThresholdRaw, out var decimalPriceThreshold);;
            
            var paths = rawPaths.Split(";");
            foreach (var path in paths)
            {
                try
                {
                    var splitPath = path.Split('-');
                    var departurePoint = splitPath[0];
                    var destination = splitPath[1];
                
                    var rawFlightInfo = await _amadeusClient.GetFlightPriceAsync(departurePoint, destination, flightDateRaw);
            
                    if (rawFlightInfo == null)
                        throw new Exception($"No flight info found for {departurePoint} to {destination} on {flightDateRaw}.");
            
                    var allFlights = _flightInfoMapper.Map(rawFlightInfo.Data);
                    var candidateFLights = new List<FlightInfo>();

                    foreach (var flight in allFlights)
                    {
                        if (flight.Price <= decimalPriceThreshold)
                            candidateFLights.Add(flight);
                    }
                
                    var textMessage = _whatsAppNotificationBuilder.BuildNotificationMessage(candidateFLights);
                
                    var sent = await _whatsAppClient.SendPriceAlertAsync(textMessage);
                
                    if (!sent)
                        _logger.LogError("Failed to send WhatsApp message.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}