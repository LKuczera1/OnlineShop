using System.Net;

namespace UnitTests.REST_API_Tests
{
    public class RESTAPI_Tests : Tests_Base
    {
        private readonly Identity_Tests.Identity_Tests _identity;
        private readonly Catalog_Tests.Catalog_Tests _catalog;
        private readonly Shopping_Tests.Shopping_Tests _shopping;

        private readonly int _delayMs;

        public RESTAPI_Tests(string baseUrl, HttpClient? http = null, int delayMs = 200)
            : base(baseUrl, http)
        {
            _delayMs = delayMs;
            _identity = new Identity_Tests.Identity_Tests("https://localhost:7002/", Http);
            _catalog  = new Catalog_Tests.Catalog_Tests("https://localhost:7001", Http);
            _shopping = new Shopping_Tests.Shopping_Tests("https://localhost:7003", Http);
        }

        public override async Task PerformTestsAsync()
        {
            Console.WriteLine("=== REST API TESTS START ===");

            // 1) Identity first: register -> login -> obtain JWT
            Console.WriteLine("[1/3] Identity tests…");
            var jwt = await _identity.PerformAndReturnJwtAsync();
            Console.WriteLine($"> JWT acquired: {(string.IsNullOrEmpty(jwt) ? "NO" : "YES")}");

            if (string.IsNullOrEmpty(jwt))
            {
                Console.WriteLine("!! Could not obtain JWT. Aborting further tests.");
                return;
            }

            // IMPORTANT: Role elevation note
            Console.WriteLine();
            Console.WriteLine("NOTE: Change the created test account's role to ADMIN in the database in 5 sec.");
            Console.WriteLine("      Some endpoints require Admin; subsequent tests assume Admin token.");
            Console.WriteLine();


            await DelayAsync(5000);

            // Apply token to subsequent test suites
            _catalog.SetJwt(jwt);
            _shopping.SetJwt(jwt);
            this.SetJwt(jwt);

            await DelayAsync(_delayMs);

            // 2) Catalog
            Console.WriteLine("[2/3] Catalog tests…");
            await _catalog.PerformTestsAsync();

            await DelayAsync(_delayMs);

            // 3) Shopping
            Console.WriteLine("[3/3] Shopping tests…");
            await _shopping.PerformTestsAsync();

            Console.WriteLine("=== REST API TESTS DONE ===");
        }
    }
}
