using System.Text.Json.Serialization;

namespace EnergiDataService.Client.Models;

/// <summary>
/// Represents the complete response from the Day Ahead Prices API
/// </summary>
/// <param name="Total">Total number of available records</param>
/// <param name="Filters">Applied filters as a JSON string</param>
/// <param name="Limit">Limit applied to the query</param>
/// <param name="Dataset">The dataset name</param>
/// <param name="Records">Collection of day-ahead price records</param>
public record DayAheadPriceResponse(
    [property: JsonPropertyName("total")]
    int Total,
    
    [property: JsonPropertyName("filters")]
    string Filters,
    
    [property: JsonPropertyName("limit")]
    int Limit,
    
    [property: JsonPropertyName("dataset")]
    string Dataset,
    
    [property: JsonPropertyName("records")]
    IReadOnlyList<DayAheadPriceRecord> Records
);