using Notifly.Service.Models;
using Notlify.Core00.Models.Amadeus;

namespace Notifly.Mappers;

public interface IFlightInfoMapper
{
    List<FlightInfo> Map(List<FlightOffer> flightOffers);
}

public class FlightInfoMapper
{
    public List<FlightInfo> Map(List<FlightOffer> flightOffers)
    {
        return new List<FlightInfo>();
    }
}