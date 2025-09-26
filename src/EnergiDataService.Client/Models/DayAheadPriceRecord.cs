using System.Text.Json.Serialization;

namespace EnergiDataService.Client.Models;

/// <summary>
/// Represents a single day-ahead price record from the Energi Data Service
/// </summary>
/// <param name="TimeUtc">The UTC timestamp for this price record</param>
/// <param name="TimeDk">The Danish local timestamp for this price record</param>
/// <param name="PriceArea">The price area (e.g., DK1, DK2)</param>
/// <param name="DayAheadPriceEur">Day-ahead price in EUR per MWh</param>
/// <param name="DayAheadPriceDkk">Day-ahead price in DKK per MWh</param>
public record DayAheadPriceRecord(
    [property: JsonPropertyName("TimeUTC")]
    DateTime TimeUtc,
    
    [property: JsonPropertyName("TimeDK")]
    DateTime TimeDk,
    
    [property: JsonPropertyName("PriceArea")]
    string PriceArea,
    
    [property: JsonPropertyName("DayAheadPriceEUR")]
    decimal DayAheadPriceEur,
    
    [property: JsonPropertyName("DayAheadPriceDKK")]
    decimal DayAheadPriceDkk
);