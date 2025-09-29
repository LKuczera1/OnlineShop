using UnitTests.REST_API_Tests;

int delayMs = 200;
Console.WriteLine($"Delay ms between steps: {delayMs}");

var runner = new RESTAPI_Tests(string.Empty, delayMs: delayMs);
await runner.PerformTestsAsync();

Console.WriteLine("Done.");