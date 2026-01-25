namespace Notlify.Core00.Interfaces;

public interface IAmadeusClient
{
    Task<decimal?> GetFlightPriceAsync(string origin, string destination, string departureDate);
}