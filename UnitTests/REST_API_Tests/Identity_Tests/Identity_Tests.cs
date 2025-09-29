using System.Net;
using System.Text.Json;

namespace UnitTests.REST_API_Tests.Identity_Tests
{
    internal sealed class Identity_Tests : Tests_Base
    {
        public Identity_Tests(string baseUrl, HttpClient? http = null) : base(baseUrl, http) { }

        // Adjust these to your actual Identity endpoints:
        private const string RegisterPath = "/api/Accounts/register"; // or /api/Account/register
        private const string LoginPath    = "/api/Accounts/login";    // or /api/Account/login

        private record RegisterRequest(string Username, string Password, string Email, string PhoneNumber, string Address, string city);
        private record LoginRequest(string Username, string Password);
        private record LoginResponse(string? token, string? accessToken, string? jwt, string? role);

        public override async Task PerformTestsAsync()
        {
            // This suite is orchestrated by RESTAPI_Tests.PerformTestsAsync()
            // Here we keep per-endpoint private methods.
            await Task.CompletedTask;
        }

        public async Task<string?> PerformAndReturnJwtAsync()
        {
            var email = $"student.test+{Guid.NewGuid():N}@example.com";
            var password = "Pa$$w0rd!";

            Console.WriteLine($"> Registering test account: {email}");
            await Register_User_ShouldReturn201(email, password);

            Console.WriteLine($"> Logging in: {email}");
            var jwt = await Login_ShouldReturnJwt(email, password);

            return jwt;
        }

        // ---- Private, one per endpoint ----

        private async Task Register_User_ShouldReturn201(string email, string password)
        {
            var req = new RegisterRequest(email, password, email, email, email, email);
            var res = await PostJsonAsync(RegisterPath, req);
            if (res.StatusCode != HttpStatusCode.Created && res.StatusCode != HttpStatusCode.OK)
            {
                var body = await res.Content.ReadAsStringAsync();
                throw new Exception($"/register failed: {(int)res.StatusCode} {res.StatusCode}\n{body}");
            }
            Console.WriteLine("  ✓ Registered (201/200)");
        }

        private async Task<string?> Login_ShouldReturnJwt(string email, string password)
        {
            var req = new LoginRequest(email, password);
            var res = await PostJsonAsync(LoginPath, req);
            if (res.StatusCode != HttpStatusCode.OK)
            {
                var body = await res.Content.ReadAsStringAsync();
                throw new Exception($"/login failed: {(int)res.StatusCode} {res.StatusCode}\n{body}");
            }

            var json = await res.Content.ReadAsStringAsync();
            try
            {
                var obj = JsonSerializer.Deserialize<LoginResponse>(json, JsonOpts);
                // many APIs use different property names; try common ones
                var token = obj?.token ?? obj?.accessToken ?? obj?.jwt;
                if (string.IsNullOrWhiteSpace(token))
                {
                    // maybe raw { "token":".." } or { "jwt":".." }
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("token", out var t)) token = t.GetString();
                    else if (doc.RootElement.TryGetProperty("jwt", out var j)) token = j.GetString();
                    else if (doc.RootElement.TryGetProperty("accessToken", out var a)) token = a.GetString();
                }
                if (string.IsNullOrWhiteSpace(token))
                    throw new Exception("Login succeeded but no token field was found in response.");

                Console.WriteLine("  ✓ Logged in (200), token acquired");
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not parse login response for JWT. Raw: " + json, ex);
            }
        }
    }
}
