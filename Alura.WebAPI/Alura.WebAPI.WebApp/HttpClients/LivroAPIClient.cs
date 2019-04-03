using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroAPIClient
    {
        private readonly IHttpContextAccessor _acessor;
        private readonly HttpClient _httpClient;

        public LivroAPIClient(HttpClient httpClient, IHttpContextAccessor acessor)
        {
            _acessor = acessor;
            _httpClient = httpClient;
        }

        private void AddBearerToken()
        {
            var token = _acessor.HttpContext.User.Claims.First(c => c.Type == "Token").Value;
            _httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"ListasLeitura/{tipo}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<Lista>();
        }
        
        public async Task<LivroApi> GetLivroAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Livros/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte []> GetCapaLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"Livros/{id}/capa");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task DeleteLivroAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"Livros/{id}");
            response.EnsureSuccessStatusCode();
        }

        private string EnvolveComAspasDuplas(string valor) => $"\"{valor}\"";
        

        private HttpContent CreateMultipartFormDataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Titulo), EnvolveComAspasDuplas("titulo"));
            content.Add(new StringContent(model.Lista.ParaString()), EnvolveComAspasDuplas("lista"));

            if (model.Id > 0)            
                content.Add(new StringContent(model.Id.ToString()), EnvolveComAspasDuplas("id"));

            if (!string.IsNullOrEmpty(model.Subtitulo))
                content.Add(new StringContent(model.Subtitulo), EnvolveComAspasDuplas("subtitulo"));

            if (!string.IsNullOrEmpty(model.Resumo))
                content.Add(new StringContent(model.Resumo), EnvolveComAspasDuplas("resumo"));

            if (!string.IsNullOrEmpty(model.Autor))
                content.Add(new StringContent(model.Autor), EnvolveComAspasDuplas("autor"));

            if (model.Capa != null)
            {
                var imageContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imageContent.Headers.Add("content-type", "image/png");
                content.Add(
                    imageContent,
                    EnvolveComAspasDuplas("capa"),
                    EnvolveComAspasDuplas("capa.png")
                );
            }

            return content;
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            AddBearerToken();
            HttpContent content = CreateMultipartFormDataContent(model);
            HttpResponseMessage response = await _httpClient.PostAsync("Livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            AddBearerToken();
            HttpContent content = CreateMultipartFormDataContent(model);
            HttpResponseMessage response = await _httpClient.PutAsync("Livros", content);
            response.EnsureSuccessStatusCode();
        }
    }
}