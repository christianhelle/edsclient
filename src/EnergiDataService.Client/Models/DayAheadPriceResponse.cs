using System.Text.Json.Serialization;

namespace EnergiDataService.Client.Models;

/// <summary>
/// Represents the complete response from the Day Ahead Prices API
/// </summary>
public class DayAheadPriceResponse
{
    /// <summary>
    /// Total number of available records
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
    
    /// <summary>
    /// Applied filters as a JSON string
    /// </summary>
    [JsonPropertyName("filters")]
    public string Filters { get; set; } = string.Empty;
    
    /// <summary>
    /// Limit applied to the query
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
    
    /// <summary>
    /// The dataset name
    /// </summary>
    [JsonPropertyName("dataset")]
    public string Dataset { get; set; } = string.Empty;
    
    /// <summary>
    /// Collection of day-ahead price records
    /// </summary>
    [JsonPropertyName("records")]
    public IReadOnlyList<DayAheadPriceRecord> Records { get; set; } = new List<DayAheadPriceRecord>();
}