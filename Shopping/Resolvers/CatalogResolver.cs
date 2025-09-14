namespace Shopping.Resolvers
{
    public class CatalogResolver
    {
        private readonly HttpClient _httpClient;
        public CatalogResolver(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public CatalogResolver()
        {

        }

        public async Task<Catalog.Dtos.ProductDto> ResolveForProduct(int productId)
        {
            return null;
        }
        /*
        public async Task<AnimalDto?> ResolveAnimal(int animalId)
        {
            var url = $"http://localhost:5241/api/Animals/{animalId}";

            try
            {
                var animal = await _httpClient.GetFromJsonAsync<AnimalDto>(url);
                return animal;
            }
            catch
            {
                return null;
            }
        }*/
    }
}
