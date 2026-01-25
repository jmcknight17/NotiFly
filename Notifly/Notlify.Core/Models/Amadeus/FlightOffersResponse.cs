using System.Text.Json.Serialization;

namespace Notlify.Core00.Models.Amadeus;

public class FlightOffersResponse
{
    [JsonPropertyName("meta")] public MetaData Meta { get; set; }

    [JsonPropertyName("data")] public List<FlightOffer> Data { get; set; }

    [JsonPropertyName("dictionaries")] public Dictionaries Dictionaries { get; set; }
}

public class MetaData
{
    [JsonPropertyName("count")] public int Count { get; set; }
}


public class FlightOffer
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("source")] public string Source { get; set; }

    [JsonPropertyName("numberOfBookableSeats")]
    public int NumberOfBookableSeats { get; set; }

    [JsonPropertyName("lastTicketingDate")]
    public string LastTicketingDate { get; set; }

    [JsonPropertyName("itineraries")] public List<Itinerary> Itineraries { get; set; }

    [JsonPropertyName("price")] public PriceInfo Price { get; set; }

    [JsonPropertyName("pricingOptions")] public PricingOptions PricingOptions { get; set; }

    [JsonPropertyName("validatingAirlineCodes")]
    public List<string> ValidatingAirlineCodes { get; set; }

    [JsonPropertyName("travelerPricings")] public List<TravelerPricing> TravelerPricings { get; set; }
}

public class Itinerary
{
    [JsonPropertyName("duration")] public string Duration { get; set; } // e.g., "PT12H40M"

    [JsonPropertyName("segments")] public List<Segment> Segments { get; set; }
}

public class Segment
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("departure")] public FlightEndpoint Departure { get; set; }

    [JsonPropertyName("arrival")] public FlightEndpoint Arrival { get; set; }

    [JsonPropertyName("carrierCode")] public string CarrierCode { get; set; }

    [JsonPropertyName("number")] public string Number { get; set; }

    [JsonPropertyName("aircraft")] public AircraftEquipment Aircraft { get; set; }

    [JsonPropertyName("duration")] public string Duration { get; set; }

    [JsonPropertyName("numberOfStops")] public int NumberOfStops { get; set; }
}

public class FlightEndpoint
{
    [JsonPropertyName("iataCode")] public string IataCode { get; set; }

    [JsonPropertyName("terminal")] public string Terminal { get; set; }

    [JsonPropertyName("at")] public DateTime At { get; set; }
}

public class AircraftEquipment
{
    [JsonPropertyName("code")] public string Code { get; set; }
}


public class PriceInfo
{
    [JsonPropertyName("currency")] public string Currency { get; set; }

    [JsonPropertyName("total")] public string Total { get; set; }

    [JsonPropertyName("base")] public string Base { get; set; }

    [JsonPropertyName("grandTotal")] public string GrandTotal { get; set; }

    [JsonPropertyName("fees")] public List<Fee> Fees { get; set; }

    public decimal GrandTotalAmount => decimal.TryParse(GrandTotal, out var d) ? d : 0m;
}

public class Fee
{
    [JsonPropertyName("amount")] public string Amount { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }
}

public class PricingOptions
{
    [JsonPropertyName("fareType")] public List<string> FareType { get; set; }

    [JsonPropertyName("includedCheckedBagsOnly")]
    public bool IncludedCheckedBagsOnly { get; set; }
}

public class TravelerPricing
{
    [JsonPropertyName("travelerId")] public string TravelerId { get; set; }

    [JsonPropertyName("fareOption")] public string FareOption { get; set; }

    [JsonPropertyName("travelerType")] public string TravelerType { get; set; }

    [JsonPropertyName("price")] public PriceInfo Price { get; set; }

    [JsonPropertyName("fareDetailsBySegment")]
    public List<FareDetails> FareDetailsBySegment { get; set; }
}

public class FareDetails
{
    [JsonPropertyName("segmentId")] public string SegmentId { get; set; }

    [JsonPropertyName("cabin")] public string Cabin { get; set; }

    [JsonPropertyName("fareBasis")] public string FareBasis { get; set; }

    [JsonPropertyName("class")] public string Class { get; set; }

    [JsonPropertyName("includedCheckedBags")]
    public BagAllowance IncludedCheckedBags { get; set; }

    [JsonPropertyName("amenities")] public List<Amenity> Amenities { get; set; }
}

public class BagAllowance
{
    [JsonPropertyName("quantity")] public int Quantity { get; set; }
}

public class Amenity
{
    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("isChargeable")] public bool IsChargeable { get; set; }
}


public class Dictionaries
{
    [JsonPropertyName("locations")] public Dictionary<string, LocationDetail> Locations { get; set; }

    [JsonPropertyName("aircraft")] public Dictionary<string, string> Aircraft { get; set; }

    [JsonPropertyName("currencies")] public Dictionary<string, string> Currencies { get; set; }

    [JsonPropertyName("carriers")] public Dictionary<string, string> Carriers { get; set; }
}

public class LocationDetail
{
    [JsonPropertyName("cityCode")] public string CityCode { get; set; }

    [JsonPropertyName("countryCode")] public string CountryCode { get; set; }
}