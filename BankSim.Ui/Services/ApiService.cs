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




        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
        
            if (_baseUrl == null)
                throw new Exception("API base URL bulunamadı.");

            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + endpoint);

            var token = _http.HttpContext?.Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await _client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            Console.WriteLine("POST edilen veri: " + JsonConvert.SerializeObject(data));
            if (_baseUrl == null)
                throw new Exception("API base URL bulunamadı.");

            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + endpoint);

            var token = _http.HttpContext?.Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return await _client.SendAsync(request);
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

