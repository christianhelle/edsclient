# Energi Data Service for .NET

A .NET client library for consuming day-ahead energy prices from the Danish [Energi Data Service](https://www.energidataservice.dk/) API.

[![NuGet](https://img.shields.io/nuget/v/EnergiDataService.Client.svg)](https://www.nuget.org/packages/EnergiDataService.Client)
[![Build Status](https://github.com/christianhelle/edsclient/workflows/Build/badge.svg)](https://github.com/christianhelle/edsclient/actions)

## Features

- **Simple API**: Clean and intuitive methods for retrieving day-ahead prices
- **Flexible**: Support for single or multiple price areas (DK1, DK2, etc.)
- **Async/Await**: Full async support with cancellation tokens
- **Dependency Injection**: Built-in support for Microsoft.Extensions.DependencyInjection
- **Comprehensive**: Strongly typed models with full XML documentation
- **Tested**: Comprehensive unit tests and integration tests

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package EnergiDataService.Client
```

Or via Package Manager Console:

```powershell
Install-Package EnergiDataService.Client
```

## Quick Start

### Basic Usage

```csharp
using EnergiDataService.Client;

// Create an HTTP client and the service client
using var httpClient = new HttpClient();
var client = new EnergiDataServiceClient(httpClient);

// Get day-ahead prices for DK1 area
var prices = await client.GetDayAheadPricesAsync("DK1", limit: 24);

Console.WriteLine($"Found {prices.Total} price records");
foreach (var record in prices.Records)
{
    Console.WriteLine($"{record.TimeDk:yyyy-MM-dd HH:mm}: {record.DayAheadPriceDkk:F2} DKK/MWh");
}
```

### Multiple Price Areas

```csharp
// Get prices for both DK1 and DK2
var prices = await client.GetDayAheadPricesAsync(new[] { "DK1", "DK2" }, limit: 50);

var dk1Prices = prices.Records.Where(r => r.PriceArea == "DK1").ToList();
var dk2Prices = prices.Records.Where(r => r.PriceArea == "DK2").ToList();
```

### With Dependency Injection

First, register the service in your DI container:

```csharp
using EnergiDataService.Client.Extensions;

// In Program.cs or Startup.cs
services.AddEnergiDataServiceClient();

// Or with custom HTTP client configuration
services.AddEnergiDataServiceClient(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

Then inject and use the client:

```csharp
public class EnergyService
{
    private readonly EnergiDataServiceClient _client;

    public EnergyService(EnergiDataServiceClient client)
    {
        _client = client;
    }

    public async Task<decimal> GetCurrentPriceAsync()
    {
        var result = await _client.GetDayAheadPricesAsync("DK1", limit: 1);
        return result.Records.First().DayAheadPriceDkk;
    }
}
```

## API Reference

### EnergiDataServiceClient Methods

#### `GetDayAheadPricesAsync(string priceArea, int limit = 100, CancellationToken cancellationToken = default)`

Gets day-ahead prices for a single price area.

**Parameters:**

- `priceArea`: Price area code (e.g., "DK1", "DK2")
- `limit`: Maximum number of records to retrieve (default: 100)
- `cancellationToken`: Cancellation token

**Returns:** `Task<DayAheadPriceResponse>`

#### `GetDayAheadPricesAsync(IEnumerable<string> priceAreas, int limit = 100, CancellationToken cancellationToken = default)`

Gets day-ahead prices for multiple price areas.

**Parameters:**

- `priceAreas`: Collection of price area codes
- `limit`: Maximum number of records to retrieve (default: 100)
- `cancellationToken`: Cancellation token

**Returns:** `Task<DayAheadPriceResponse>`

### Data Models

#### `DayAheadPriceResponse`

Represents the complete response from the API:

```csharp
public class DayAheadPriceResponse
{
    public int Total { get; set; }                                    // Total number of available records
    public string Filters { get; set; }                              // Applied filters as JSON string
    public int Limit { get; set; }                                   // Limit applied to the query
    public string Dataset { get; set; }                              // Dataset name ("DayAheadPrices")
    public IReadOnlyList<DayAheadPriceRecord> Records { get; set; }  // Collection of price records
}
```

#### `DayAheadPriceRecord`

Represents a single day-ahead price record:

```csharp
public class DayAheadPriceRecord
{
    public DateTime TimeUtc { get; set; }           // UTC timestamp
    public DateTime TimeDk { get; set; }            // Danish local timestamp
    public string PriceArea { get; set; }           // Price area (e.g., "DK1", "DK2")
    public decimal DayAheadPriceEur { get; set; }   // Price in EUR per MWh
    public decimal DayAheadPriceDkk { get; set; }   // Price in DKK per MWh
}
```

## Price Areas

The Danish electricity market is divided into different price areas:

- **DK1**: Western Denmark (Jutland and Funen)
- **DK2**: Eastern Denmark (Zealand, Lolland, Falster, and Bornholm)

You can also use price areas from other Nordic countries if available in the API.

## Error Handling

The client throws the following exceptions:

- `ArgumentNullException`: When required parameters are null
- `ArgumentException`: When price areas list is empty or invalid
- `HttpRequestException`: When HTTP requests fail
- `InvalidOperationException`: When response cannot be deserialized

```csharp
try
{
    var prices = await client.GetDayAheadPricesAsync("DK1");
    // Process prices...
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP error: {ex.Message}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid argument: {ex.Message}");
}
```

## Configuration

### HTTP Client Configuration

When using dependency injection, you can configure the underlying HTTP client:

```csharp
services.AddEnergiDataServiceClient(client =>
{
    client.Timeout = TimeSpan.FromMinutes(1);
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
});
```

### Retry Policies

For production use, consider adding retry policies using libraries like Polly:

```csharp
services.AddHttpClient<EnergiDataServiceClient>()
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(30))
    .AddPolicyHandler(Policy
        .Handle<HttpRequestException>()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

## Examples

### Get Today's Hourly Prices

```csharp
var today = DateTime.Today;
var prices = await client.GetDayAheadPricesAsync("DK1", limit: 100);

var todaysPrices = prices.Records
    .Where(r => r.TimeDk.Date == today)
    .OrderBy(r => r.TimeDk)
    .ToList();

foreach (var price in todaysPrices)
{
    Console.WriteLine($"{price.TimeDk:HH:mm}: {price.DayAheadPriceDkk:F2} øre/kWh");
}
```

### Find Peak and Off-Peak Hours

```csharp
var prices = await client.GetDayAheadPricesAsync("DK1", limit: 24);

var sortedPrices = prices.Records
    .OrderBy(r => r.DayAheadPriceDkk)
    .ToList();

var cheapestHour = sortedPrices.First();
var expensiveHour = sortedPrices.Last();

Console.WriteLine($"Cheapest: {cheapestHour.TimeDk:HH:mm} - {cheapestHour.DayAheadPriceDkk:F2} DKK/MWh");
Console.WriteLine($"Most expensive: {expensiveHour.TimeDk:HH:mm} - {expensiveHour.DayAheadPriceDkk:F2} DKK/MWh");
```

### Compare Price Areas

```csharp
var prices = await client.GetDayAheadPricesAsync(new[] { "DK1", "DK2" }, limit: 50);

var dk1Average = prices.Records
    .Where(r => r.PriceArea == "DK1")
    .Average(r => r.DayAheadPriceDkk);

var dk2Average = prices.Records
    .Where(r => r.PriceArea == "DK2")
    .Average(r => r.DayAheadPriceDkk);

Console.WriteLine($"DK1 average: {dk1Average:F2} DKK/MWh");
Console.WriteLine($"DK2 average: {dk2Average:F2} DKK/MWh");
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Building from Source

```bash
git clone https://github.com/christianhelle/edsclient.git
cd edsclient
dotnet build
dotnet test
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Data provided by [Energi Data Service](https://www.energidataservice.dk/) (Energinet)
- Built with .NET 8 and modern C# features

## Related Links

- [Energi Data Service API Documentation](https://www.energidataservice.dk/tso-electricity/dayaheadprices)
- [Danish Energy Agency](https://ens.dk)
- [Energinet](https://www.energinet.dk)

