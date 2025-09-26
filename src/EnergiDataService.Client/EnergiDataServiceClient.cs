using EnergiDataService.Client.Models;
using System.Text.Json;
using System.Web;

namespace EnergiDataService.Client;

/// <summary>
/// Client for retrieving data from Energi Data Service
/// </summary>
public class EnergiDataServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string BaseUrl = "https://api.energidataservice.dk";

    /// <summary>
    /// Initializes a new instance of the EnergiDataServiceClient
    /// </summary>
    /// <param name="httpClient">HttpClient instance for making requests</param>
    public EnergiDataServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Gets day-ahead prices for the specified price areas
    /// </summary>
    /// <param name="priceAreas">List of price areas to retrieve (e.g., "DK1", "DK2")</param>
    /// <param name="limit">Maximum number of records to retrieve (default: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Day-ahead price response</returns>
    /// <exception cref="ArgumentNullException">Thrown when priceAreas is null</exception>
    /// <exception cref="ArgumentException">Thrown when priceAreas is empty</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails</exception>
    public async Task<DayAheadPriceResponse> GetDayAheadPricesAsync(
        IEnumerable<string> priceAreas,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        if (priceAreas == null)
            throw new ArgumentNullException(nameof(priceAreas));

        var priceAreaList = priceAreas.ToList();
        if (!priceAreaList.Any())
            throw new ArgumentException("At least one price area must be specified", nameof(priceAreas));

        var filter = JsonSerializer.Serialize(new { PriceArea = priceAreaList });
        var encodedFilter = HttpUtility.UrlEncode(filter);
        
        var url = $"{BaseUrl}/dataset/DayAheadPrices?filter={encodedFilter}&limit={limit}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<DayAheadPriceResponse>(content, _jsonOptions);

        return result ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Gets day-ahead prices for a single price area
    /// </summary>
    /// <param name="priceArea">Price area to retrieve (e.g., "DK1", "DK2")</param>
    /// <param name="limit">Maximum number of records to retrieve (default: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Day-ahead price response</returns>
    public async Task<DayAheadPriceResponse> GetDayAheadPricesAsync(
        string priceArea,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(priceArea))
            throw new ArgumentException("Price area cannot be null or empty", nameof(priceArea));

        return await GetDayAheadPricesAsync(new[] { priceArea }, limit, cancellationToken);
    }
}
