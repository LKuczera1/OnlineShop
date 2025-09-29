using System.Net;

namespace UnitTests.REST_API_Tests.Catalog_Tests
{
    internal sealed class Catalog_Tests : Tests_Base
    {
        public Catalog_Tests(string baseUrl, HttpClient? http = null) : base(baseUrl, http) { }

        // Adjust these to your actual endpoints
        private const string ProductsPath = "/api/Products";
        private const string ProductByIdPath = "/api/Products/{0}"; // string.Format

        public override async Task PerformTestsAsync()
        {
            var steps = new List<Func<Task>>
            {
                Get_Products_ShouldReturn200,
                Create_Product_AsAdmin_ShouldReturn201,
                Get_ProductById_ShouldReturn200,
                Update_Product_AsAdmin_ShouldReturn204,
                Delete_Product_AsAdmin_ShouldReturn204
            };

            foreach (var step in steps)
            {
                try
                {
                    await step();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  ✗ " + ex.Message);
                    throw;
                }
            }
        }

        // ---- Private, one per endpoint ----

        private async Task Get_Products_ShouldReturn200()
        {
            var res = await GetAsync(ProductsPath);
            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/Products GET failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ GET /Products (200)");
        }

        private async Task Create_Product_AsAdmin_ShouldReturn201()
        {
            // minimal payload — adjust fields to your DTO
            var dto = new { Name = "Test Product", Price = 9.99, Description = "Auto-generated" };
            var res = await PostJsonAsync(ProductsPath, dto);
            if (res.StatusCode != HttpStatusCode.Created && res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/Products POST failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ POST /Products (201/200)");
        }

        private async Task Get_ProductById_ShouldReturn200()
        {
            // naive: ask list, pick first id
            var listRes = await GetAsync(ProductsPath);
            var body = await listRes.Content.ReadAsStringAsync();
            var id = ExtractFirstId(body) ?? 1;
            var res = await GetAsync(string.Format(ProductByIdPath, id));
            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/Products/{id} GET failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ GET /Products/{id} (200)");
        }

        private async Task Update_Product_AsAdmin_ShouldReturn204()
        {
            var listRes = await GetAsync(ProductsPath);
            var body = await listRes.Content.ReadAsStringAsync();
            var id = ExtractFirstId(body) ?? 1;
            var dto = new { Name = "Updated name", Price = 19.99 };
            var res = await PutJsonAsync(string.Format(ProductByIdPath, id), dto);
            if (res.StatusCode != HttpStatusCode.NoContent && res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/Products PUT failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ PUT /Products/{id} (204/200)");
        }

        private async Task Delete_Product_AsAdmin_ShouldReturn204()
        {
            var listRes = await GetAsync(ProductsPath);
            var body = await listRes.Content.ReadAsStringAsync();
            var id = ExtractFirstId(body) ?? 1;
            var res = await DeleteAsync(string.Format(ProductByIdPath, id));
            if (res.StatusCode != HttpStatusCode.NoContent && res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/Products DELETE failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ DELETE /Products/{id} (204/200)");
        }

        // naive helper: tries to read an "id" number from JSON using a simple heuristic
        private static int? ExtractFirstId(string json)
        {
            // very lightweight: find "id": number
            var idx = json.IndexOf("\"id\"", StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;
            var sub = json.Substring(idx);
            var digits = new string(sub.SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(digits, out var id)) return id;
            return null;
        }
    }
}
