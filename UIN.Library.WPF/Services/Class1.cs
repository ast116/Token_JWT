using System.Net.Http;
using System.Net.Http.Json;

namespace UIN.Library.WPF.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7223/");
        }

        public async Task<bool> Login(string username, string role)
        {
            var response = await _httpClient.PostAsync(
                $"api/auth/login?username={username}&role={role}", null);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            AccessToken = result.accessToken;
            RefreshToken = result.refreshToken;

            return true;
        }

        public async Task<List<Livre>> GetBooks()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await _httpClient.GetAsync("api/books");

            // Si token expiré
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshAccessToken();

                if (!refreshed)
                    throw new Exception("Session expirée, reconnectez-vous");

                // Rejouer la requête avec nouveau token
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

                response = await _httpClient.GetAsync("api/books");
            }

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erreur API");

            return await response.Content.ReadFromJsonAsync<List<Livre>>();
        }

        public async Task<bool> RefreshAccessToken()
        {
            var response = await _httpClient.PostAsync(
                $"api/auth/refresh?refreshToken={RefreshToken}", null);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            AccessToken = result.accessToken;

            return true;
        }

        public async Task Logout()
        {
            if (string.IsNullOrEmpty(RefreshToken))
                return;

            await _httpClient.PostAsync(
                $"api/auth/logout?refreshToken={RefreshToken}", null);

            // Nettoyage côté client
            AccessToken = null;
            RefreshToken = null;
        }
    }

    public class LoginResponse
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }

    public class Livre
    {
        public string titre { get; set; }
        public string auteur { get; set; }

        public override string ToString()
        {
            return $"{titre} - {auteur}";
        }

    }

}