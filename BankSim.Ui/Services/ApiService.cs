using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BankSim.Ui.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _http;
        private readonly string? _baseUrl;

        public ApiService(HttpClient client, IHttpContextAccessor http, IConfiguration configuration)
        {
            _client = client;
            _http = http;
            _baseUrl = configuration["ApiBaseUrl"];
        }

        private void SetAuthorizationHeader()
        {
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            else
                _client.DefaultRequestHeaders.Authorization = null;
        }

        public string GetToken()
        {
            return _http.HttpContext?.Session.GetString("token") ?? "";
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            SetAuthorizationHeader();
            if (_baseUrl == null)
                throw new Exception("API base URL bulunamadı.");
            return await _client.GetAsync(_baseUrl + endpoint);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            SetAuthorizationHeader();
            if (_baseUrl == null)
                throw new Exception("API base URL bulunamadı.");
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PostAsync(_baseUrl + endpoint, content);
        }

        
        public async Task<string?> GetErrorMessageAsync(HttpResponseMessage response)
        {
            if (response.Content == null)
                return null;
            var errorString = await response.Content.ReadAsStringAsync();
            try
            {
                var error = JsonConvert.DeserializeObject<ApiErrorResponse>(errorString);
                return error?.message;
            }
            catch
            {
                return errorString; 
            }
        }

        private class ApiErrorResponse
        {
            public string? message { get; set; }
        }
    }
}
