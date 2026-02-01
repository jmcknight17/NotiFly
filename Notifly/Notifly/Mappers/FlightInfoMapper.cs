using Microsoft.Extensions.Configuration;
using Notifly.Service.Models;
using Notlify.Core00.Helpers;
using Notlify.Core00.Interfaces;
using Notlify.Core00.Models.Amadeus;

namespace Notifly.Mappers;

public interface IFlightInfoMapper
{
    List<FlightInfo> Map(List<FlightOffer> flightOffers);
}

public class FlightInfoMapper : IFlightInfoMapper
{
    private readonly IAmadeusClient _amadeusClient;

    public FlightInfoMapper(IAmadeusClient amadeusClient)
    {
        _amadeusClient = amadeusClient;
    }

    public List<FlightInfo> Map(List<FlightOffer> flightOffers)
    {
        var flightInfoList = new List<FlightInfo>();

        foreach (var offer in flightOffers)
        {
            var flightInfo = new FlightInfo
            {
                Price = offer.Price.GrandTotalAmount,
                Currency = offer.Price.Currency,
                Airline = AirlineReference.GetAirlineName(offer.ValidatingAirlineCodes.First()),
                stops = offer.Itineraries.First().Segments.Count - 1,
                Duration = offer.Itineraries.First().Duration
            };
            flightInfoList.Add(flightInfo);
        }
        
        return flightInfoList;
    }
}