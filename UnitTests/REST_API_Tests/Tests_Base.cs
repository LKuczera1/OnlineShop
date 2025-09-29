using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace UnitTests.REST_API_Tests
{
    public abstract class Tests_Base
    {
        protected readonly HttpClient Http;
        protected readonly string BaseUrl;
        protected string? JwtToken;

        protected static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        protected Tests_Base(string baseUrl, HttpClient? http = null)
        {
            BaseUrl = baseUrl.TrimEnd('/');
            Http = http ?? new HttpClient();
        }

        public void SetJwt(string? token)
        {
            JwtToken = token;
        }

        protected void ApplyAuthHeader()
        {
            if (!string.IsNullOrWhiteSpace(JwtToken))
            {
                Http.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", JwtToken);
            }
            else
            {
                Http.DefaultRequestHeaders.Authorization = null;
            }
        }

        protected async Task<HttpResponseMessage> GetAsync(string path)
        {
            ApplyAuthHeader();
            return await Http.GetAsync(Combine(path));
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string path)
        {
            ApplyAuthHeader();
            return await Http.DeleteAsync(Combine(path));
        }

        protected async Task<HttpResponseMessage> PostJsonAsync<T>(string path, T body)
        {
            ApplyAuthHeader();
            var json = JsonSerializer.Serialize(body, JsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await Http.PostAsync(Combine(path), content);
        }

        protected async Task<HttpResponseMessage> PutJsonAsync<T>(string path, T body)
        {
            ApplyAuthHeader();
            var json = JsonSerializer.Serialize(body, JsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await Http.PutAsync(Combine(path), content);
        }

        protected static async Task DelayAsync(int ms) => await Task.Delay(ms);

        private string Combine(string relative)
        {
            if (string.IsNullOrWhiteSpace(relative)) return BaseUrl;
            if (relative.StartsWith("http")) return relative;
            if (!relative.StartsWith("/")) relative = "/" + relative;
            return BaseUrl + relative;
        }

        public abstract Task PerformTestsAsync();
    }
}
