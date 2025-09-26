using System.Text.Json.Serialization;

namespace EnergiDataService.Client.Models;

/// <summary>
/// Represents a single day-ahead price record from the Energi Data Service
/// </summary>
public class DayAheadPriceRecord
{
    /// <summary>
    /// The UTC timestamp for this price record
    /// </summary>
    [JsonPropertyName("TimeUTC")]
    public DateTime TimeUtc { get; set; }
    
    /// <summary>
    /// The Danish local timestamp for this price record
    /// </summary>
    [JsonPropertyName("TimeDK")]
    public DateTime TimeDk { get; set; }
    
    /// <summary>
    /// The price area (e.g., DK1, DK2)
    /// </summary>
    [JsonPropertyName("PriceArea")]
    public string PriceArea { get; set; } = string.Empty;
    
    /// <summary>
    /// Day-ahead price in EUR per MWh
    /// </summary>
    [JsonPropertyName("DayAheadPriceEUR")]
    public decimal DayAheadPriceEur { get; set; }
    
    /// <summary>
    /// Day-ahead price in DKK per MWh
    /// </summary>
    [JsonPropertyName("DayAheadPriceDKK")]
    public decimal DayAheadPriceDkk { get; set; }
}