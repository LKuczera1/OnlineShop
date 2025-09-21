using Catalog.Dtos;
using static System.Net.WebRequestMethods;

namespace Shopping.Resolvers
{
    public class CatalogResolver
    {
        private readonly HttpClient _httpClient;
        private string baseurl;
        public CatalogResolver(HttpClient httpClient)
        {
            _httpClient = httpClient;

            //Temporarily hardcoded
            baseurl = "https://localhost:7001/api/";
        }

        public async Task<Catalog.Dtos.ProductDto> ResolveForProduct(int productId)
        {
            var url = baseurl + $"{productId}";

            try
            {
                var product = await _httpClient.GetFromJsonAsync<ProductDto>(url);
                return product;
            }
            catch
            {
                return null;
            }
        }
    }
}
