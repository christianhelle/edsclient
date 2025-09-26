namespace EnergiDataService.Client.Tests.Integration;

/// <summary>
/// Integration tests that make real HTTP requests to the Energi Data Service API
/// These tests require internet connectivity and may be slower than unit tests
/// </summary>
public class EnergiDataServiceClientIntegrationTests
{
    [Fact]
    public async Task GetDayAheadPricesAsync_WithRealApi_DK1_ReturnsData()
    {
        // Arrange
        using var httpClient = new HttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act
        var result = await client.GetDayAheadPricesAsync("DK1", limit: 5);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Total > 0);
        Assert.Equal("DayAheadPrices", result.Dataset);
        Assert.NotEmpty(result.Records);
        Assert.All(result.Records, record => 
        {
            Assert.Equal("DK1", record.PriceArea);
            Assert.True(record.DayAheadPriceEur > 0);
            Assert.True(record.DayAheadPriceDkk > 0);
            Assert.NotEqual(default(DateTime), record.TimeUtc);
            Assert.NotEqual(default(DateTime), record.TimeDk);
        });
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithRealApi_DK2_ReturnsData()
    {
        // Arrange
        using var httpClient = new HttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act
        var result = await client.GetDayAheadPricesAsync("DK2", limit: 5);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Total > 0);
        Assert.Equal("DayAheadPrices", result.Dataset);
        Assert.NotEmpty(result.Records);
        Assert.All(result.Records, record => 
        {
            Assert.Equal("DK2", record.PriceArea);
            Assert.True(record.DayAheadPriceEur >= 0); // Prices can be negative in some cases
            Assert.True(record.DayAheadPriceDkk != 0);
            Assert.NotEqual(default(DateTime), record.TimeUtc);
            Assert.NotEqual(default(DateTime), record.TimeDk);
        });
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithRealApi_MultiplePriceAreas_ReturnsData()
    {
        // Arrange
        using var httpClient = new HttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act
        var result = await client.GetDayAheadPricesAsync(new[] { "DK1", "DK2" }, limit: 10);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Total > 0);
        Assert.Equal("DayAheadPrices", result.Dataset);
        Assert.NotEmpty(result.Records);
        
        var dk1Records = result.Records.Where(r => r.PriceArea == "DK1").ToList();
        var dk2Records = result.Records.Where(r => r.PriceArea == "DK2").ToList();
        
        Assert.NotEmpty(dk1Records);
        Assert.NotEmpty(dk2Records);
    }

    [Fact]
    public async Task GetDayAheadPricesAsync_WithRealApi_InvalidPriceArea_ReturnsEmptyRecords()
    {
        // Arrange
        using var httpClient = new HttpClient();
        var client = new EnergiDataServiceClient(httpClient);

        // Act
        var result = await client.GetDayAheadPricesAsync("INVALID_AREA", limit: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("DayAheadPrices", result.Dataset);
        Assert.Empty(result.Records);
    }
}