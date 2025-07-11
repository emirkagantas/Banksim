using Microsoft.Extensions.Configuration;
using System.Text.Json;


namespace BankSim.Application.Utils
{
    public class ExchangeRateService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public ExchangeRateService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<decimal> GetExchangeRateAsync(string from, string to)
        {
            if (from == to)
                return 1m;
            var apiKey = _config["ExchangeRateApi:ApiKey"];
            var url = $"https://api.exchangerate.host/convert?from={from}&to={to}&amount=1&access_key={apiKey}";
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);

            if (!doc.RootElement.TryGetProperty("result", out var resultProp))
                throw new Exception("API yanıtında 'result' yok! Dönen içerik: " + content);

            var rate = resultProp.GetDecimal();
            return rate;
        }

    }
}
