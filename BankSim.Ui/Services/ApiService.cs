using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BankSim.Ui.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _http;

        public ApiService(HttpClient client, IHttpContextAccessor http)
        {
            _client = client;
            _http = http;
        }

        public async Task<string> GetToken()
        {
            return _http.HttpContext?.Session.GetString("token") ?? "";
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetToken());
            return await _client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetToken());
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PostAsync(url, content);
        }
    }

}
