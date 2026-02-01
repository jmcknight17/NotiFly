using Notlify.Core00.Models.Amadeus;

namespace Notlify.Core00.Interfaces;

public interface IAmadeusClient
{
    Task<FlightOffersResponse?> GetFlightPriceAsync(string origin, string destination, string departureDate);
}