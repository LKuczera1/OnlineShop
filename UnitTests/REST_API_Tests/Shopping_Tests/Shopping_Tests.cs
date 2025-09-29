using System.Net;

namespace UnitTests.REST_API_Tests.Shopping_Tests
{
    internal sealed class Shopping_Tests : Tests_Base
    {
        public Shopping_Tests(string baseUrl, HttpClient? http = null) : base(baseUrl, http) { }

        // Adjust these to your actual endpoints
        private const string CartPath = "/api/Cart";
        private const string WishlistPath = "/api/WishlistItems";

        public override async Task PerformTestsAsync()
        {
            var steps = new List<Func<Task>>
            {
                Get_Wishlist_AsAdminOrCustomer_ShouldReturn200,
                Add_To_Wishlist_ShouldReturn201,
                Remove_From_Wishlist_ShouldReturn204,

                Get_Cart_ShouldReturn200,
                Add_To_Cart_ShouldReturn201,
                Remove_From_Cart_ShouldReturn204
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

        private async Task Get_Wishlist_AsAdminOrCustomer_ShouldReturn200()
        {
            var res = await GetAsync(WishlistPath);
            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/WishlistItems GET failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ GET /WishlistItems (200)");
        }

        private async Task Add_To_Wishlist_ShouldReturn201()
        {
            var dto = new { ProductId = 1 }; // adjust to your DTO
            var res = await PostJsonAsync(WishlistPath, dto);
            if (res.StatusCode != HttpStatusCode.Created && res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/WishlistItems POST failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ POST /WishlistItems (201/200)");
        }

        private async Task Remove_From_Wishlist_ShouldReturn204()
        {
            // naive: delete first
            var res = await DeleteAsync(WishlistPath + "/1"); // adjust if your API uses body or query
            if (res.StatusCode != HttpStatusCode.NoContent && res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/WishlistItems DELETE failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ DELETE /WishlistItems/{id} (204/200)");
        }

        private async Task Get_Cart_ShouldReturn200()
        {
            var res = await GetAsync(CartPath);
            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/Cart GET failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ GET /Cart (200)");
        }

        private async Task Add_To_Cart_ShouldReturn201()
        {
            var dto = new { ProductId = 1, Quantity = 1 }; // adjust to your DTO
            var res = await PostJsonAsync(CartPath, dto);
            if (res.StatusCode != HttpStatusCode.Created && res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/Cart POST failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ POST /Cart (201/200)");
        }

        private async Task Remove_From_Cart_ShouldReturn204()
        {
            var res = await DeleteAsync(CartPath + "/1"); // adjust according to your routes
            if (res.StatusCode != HttpStatusCode.NoContent && res.StatusCode != HttpStatusCode.OK)
                throw new Exception($"/Cart DELETE failed: {(int)res.StatusCode} {res.StatusCode}");
            Console.WriteLine("  ✓ DELETE /Cart/{id} (204/200)");
        }
    }
}
