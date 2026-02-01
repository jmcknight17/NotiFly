namespace Notifly.Service.Models;

public class FlightInfo
{
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public string Airline { get; set; }
    public int stops { get; set; }
    public string Duration { get; set; }
}