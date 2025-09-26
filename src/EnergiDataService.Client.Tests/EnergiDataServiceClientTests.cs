using EnergiDataService.Client.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace EnergiDataService.Client.Tests;

public class EnergiDataServiceClientTests
{
    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EnergiDataServiceClient(null!));
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithSinglePriceArea_ReturnsExpectedData()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var expectedResponse = new DayAheadPriceResponse
        {
            Total = 2,
            Filters = "{\"PriceArea\":[\"DK1\"]}",
            Limit = 100,
            Dataset = "DayAheadPrices",
            Records = new List<DayAheadPriceRecord>
            {
                new DayAheadPriceRecord
                {
                    TimeUtc = DateTime.Parse("2025-09-27T21:00:00"),
                    TimeDk = DateTime.Parse("2025-09-27T23:00:00"),
                    PriceArea = "DK1",
                    DayAheadPriceEur = 99.27m,
                    DayAheadPriceDkk = 740.95m
                },
                new DayAheadPriceRecord
                {
                    TimeUtc = DateTime.Parse("2025-09-27T20:00:00"),
                    TimeDk = DateTime.Parse("2025-09-27T22:00:00"),
                    PriceArea = "DK1",
                    DayAheadPriceEur = 104.44m,
                    DayAheadPriceDkk = 779.54m
                }
            }
        };

        var responseJson = JsonSerializer.Serialize(expectedResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        mockHttp.When("https://api.energidataservice.dk/dataset/DayAheadPrices*")
                .Respond("application/json", responseJson);

        var httpClient = mockHttp.ToHttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act
        var result = await client.GetDayAheadPricesAsync("DK1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Total, result.Total);
        Assert.Equal(expectedResponse.Dataset, result.Dataset);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("DK1", result.Records.First().PriceArea);
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithMultiplePriceAreas_ReturnsExpectedData()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var expectedResponse = new DayAheadPriceResponse
        {
            Total = 2,
            Filters = "{\"PriceArea\":[\"DK1\",\"DK2\"]}",
            Limit = 100,
            Dataset = "DayAheadPrices",
            Records = new List<DayAheadPriceRecord>
            {
                new DayAheadPriceRecord
                {
                    TimeUtc = DateTime.Parse("2025-09-27T21:00:00"),
                    TimeDk = DateTime.Parse("2025-09-27T23:00:00"),
                    PriceArea = "DK1",
                    DayAheadPriceEur = 99.27m,
                    DayAheadPriceDkk = 740.95m
                },
                new DayAheadPriceRecord
                {
                    TimeUtc = DateTime.Parse("2025-09-27T21:00:00"),
                    TimeDk = DateTime.Parse("2025-09-27T23:00:00"),
                    PriceArea = "DK2",
                    DayAheadPriceEur = 102.15m,
                    DayAheadPriceDkk = 762.31m
                }
            }
        };

        var responseJson = JsonSerializer.Serialize(expectedResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        mockHttp.When("https://api.energidataservice.dk/dataset/DayAheadPrices*")
                .Respond("application/json", responseJson);

        var httpClient = mockHttp.ToHttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act
        var result = await client.GetDayAheadPricesAsync(new[] { "DK1", "DK2" });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Total, result.Total);
        Assert.Equal(2, result.Records.Count);
        Assert.Contains(result.Records, r => r.PriceArea == "DK1");
        Assert.Contains(result.Records, r => r.PriceArea == "DK2");
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithCustomLimit_PassesLimitCorrectly()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var expectedResponse = new DayAheadPriceResponse
        {
            Total = 50,
            Filters = "{\"PriceArea\":[\"DK1\"]}",
            Limit = 50,
            Dataset = "DayAheadPrices",
            Records = new List<DayAheadPriceRecord>()
        };

        var responseJson = JsonSerializer.Serialize(expectedResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Match the full URL pattern
        mockHttp.When("https://api.energidataservice.dk/dataset/DayAheadPrices*")
                .Respond("application/json", responseJson);

        var httpClient = mockHttp.ToHttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act
        var result = await client.GetDayAheadPricesAsync("DK1", limit: 50);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(50, result.Limit);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetDayAheadPricesAsync_WithInvalidSinglePriceArea_ThrowsArgumentException(string priceArea)
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var httpClient = mockHttp.ToHttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => client.GetDayAheadPricesAsync(priceArea));
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithNullPriceAreas_ThrowsArgumentNullException()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var httpClient = mockHttp.ToHttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => client.GetDayAheadPricesAsync((IEnumerable<string>)null!));
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithEmptyPriceAreas_ThrowsArgumentException()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var httpClient = mockHttp.ToHttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => client.GetDayAheadPricesAsync(Array.Empty<string>()));
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithHttpError_ThrowsHttpRequestException()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.energidataservice.dk/dataset/DayAheadPrices*")
                .Respond(HttpStatusCode.BadRequest);

        var httpClient = mockHttp.ToHttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.GetDayAheadPricesAsync("DK1"));
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithValidResponse_DeserializesCorrectly()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var responseJson = """
        {
            "total": 1,
            "filters": "{\"PriceArea\":[\"DK2\"]}",
            "limit": 100,
            "dataset": "DayAheadPrices",
            "records": [
                {
                    "TimeUTC": "2025-09-27T21:00:00",
                    "TimeDK": "2025-09-27T23:00:00",
                    "PriceArea": "DK2",
                    "DayAheadPriceEUR": 99.269997,
                    "DayAheadPriceDKK": 740.951258
                }
            ]
        }
        """;

        mockHttp.When("https://api.energidataservice.dk/dataset/DayAheadPrices*")
                .Respond("application/json", responseJson);

        var httpClient = mockHttp.ToHttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act
        var result = await client.GetDayAheadPricesAsync("DK2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Equal("DayAheadPrices", result.Dataset);
        Assert.Single(result.Records);
        
        var record = result.Records.First();
        Assert.Equal(DateTime.Parse("2025-09-27T21:00:00"), record.TimeUtc);
        Assert.Equal(DateTime.Parse("2025-09-27T23:00:00"), record.TimeDk);
        Assert.Equal("DK2", record.PriceArea);
        Assert.Equal(99.269997m, record.DayAheadPriceEur);
        Assert.Equal(740.951258m, record.DayAheadPriceDkk);
    }
}