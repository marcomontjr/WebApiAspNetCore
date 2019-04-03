using Alura.ListaLeitura.Seguranca;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
    }
    public class AuthAPIClient
    {
        private readonly HttpClient _httpClient;

        public AuthAPIClient(HttpClient httpClient) => _httpClient = httpClient;
        
        public async Task<LoginResult> PostLoginAsync(LoginModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("Login", model);

            return new LoginResult
            {
                Succeeded = response.IsSuccessStatusCode,
                Token = await response.Content.ReadAsStringAsync()
            };
        }
    }
}