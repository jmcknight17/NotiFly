using System.Text.Json.Serialization;

namespace Notlify.Core00.Models.Amadeus;

public class FlightAirlinesResponse
{
    [JsonPropertyName("meta")] public Meta? Meta { get; set; }

    [JsonPropertyName("data")] public List<Airline>? Data { get; set; }
}

public class Meta
{
    [JsonPropertyName("count")] public int Count { get; set; }

    [JsonPropertyName("links")] public Links? Links { get; set; }
}

public class Links
{
    [JsonPropertyName("self")] public string? Self { get; set; }
}

public class Airline
{
    [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("iataCode")] public string? IataCode { get; set; }

    [JsonPropertyName("icaoCode")] public string? IcaoCode { get; set; }

    [JsonPropertyName("businessName")] public string? BusinessName { get; set; }

    [JsonPropertyName("commonName")] public string? CommonName { get; set; }
}