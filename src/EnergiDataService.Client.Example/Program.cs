using EnergiDataService.Client;

Console.WriteLine("=== Energi Data Service Client Example ===");
Console.WriteLine();

// Create HTTP client and service client
using var httpClient = new HttpClient();
var client = new EnergiDataServiceClient(httpClient);

try
{
    // Example 1: Get prices for DK1
    Console.WriteLine("📊 Getting day-ahead prices for DK1 (last 24 hours)...");
    var dk1Prices = await client.GetDayAheadPricesAsync("DK1", limit: 24);
    
    Console.WriteLine($"Found {dk1Prices.Records.Count} price records");
    Console.WriteLine();

    // Display first few records
    Console.WriteLine("Latest prices:");
    foreach (var record in dk1Prices.Records.Take(5))
    {
        Console.WriteLine($"  {record.TimeDk:yyyy-MM-dd HH:mm} | " +
                         $"{record.DayAheadPriceDkk:F2} DKK/MWh | " +
                         $"{record.DayAheadPriceEur:F2} EUR/MWh");
    }

    Console.WriteLine();
    Console.WriteLine("----------------------------------------");
    Console.WriteLine();

    // Example 2: Compare DK1 and DK2
    Console.WriteLine("🔄 Comparing prices between DK1 and DK2...");
    var bothPrices = await client.GetDayAheadPricesAsync(new[] { "DK1", "DK2" }, limit: 20);

    var dk1Records = bothPrices.Records.Where(r => r.PriceArea == "DK1").ToList();
    var dk2Records = bothPrices.Records.Where(r => r.PriceArea == "DK2").ToList();

    if (dk1Records.Any() && dk2Records.Any())
    {
        var dk1Average = dk1Records.Average(r => r.DayAheadPriceDkk);
        var dk2Average = dk2Records.Average(r => r.DayAheadPriceDkk);

        Console.WriteLine($"DK1 average: {dk1Average:F2} DKK/MWh");
        Console.WriteLine($"DK2 average: {dk2Average:F2} DKK/MWh");
        Console.WriteLine($"Difference: {Math.Abs(dk1Average - dk2Average):F2} DKK/MWh");
    }

    Console.WriteLine();
    Console.WriteLine("----------------------------------------");
    Console.WriteLine();

    // Example 3: Find peak and off-peak hours
    Console.WriteLine("⚡ Finding peak and off-peak hours for DK1...");
    var dailyPrices = await client.GetDayAheadPricesAsync("DK1", limit: 48);

    if (dailyPrices.Records.Any())
    {
        var sortedPrices = dailyPrices.Records.OrderBy(r => r.DayAheadPriceDkk).ToList();
        var cheapest = sortedPrices.First();
        var expensive = sortedPrices.Last();

        Console.WriteLine($"💰 Cheapest hour: {cheapest.TimeDk:yyyy-MM-dd HH:mm} - {cheapest.DayAheadPriceDkk:F2} DKK/MWh");
        Console.WriteLine($"💸 Most expensive hour: {expensive.TimeDk:yyyy-MM-dd HH:mm} - {expensive.DayAheadPriceDkk:F2} DKK/MWh");
        Console.WriteLine($"📈 Price spread: {expensive.DayAheadPriceDkk - cheapest.DayAheadPriceDkk:F2} DKK/MWh");
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"❌ HTTP error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("✅ Example completed!");
Console.WriteLine("Press any key to exit...");
