using BankSim.Application.DTOs;
using BankSim.Application.Services;
using System.Net.Http.Json;

public class InvoiceService : IInvoiceService
{
    private readonly HttpClient _client;
    public InvoiceService(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("InvoiceAPI");
    }

    public async Task<List<InvoiceDto>> GetInvoicesAsync(string tckn)
    {
        var resp = await _client.GetAsync($"/invoices/customer/{tckn}");
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<InvoiceDto>>();
    }

    public async Task<bool> PayInvoiceAsync(PayInvoiceRequest req, string bearerToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/invoices/pay")
        {
            Content = JsonContent.Create(req)
        };

       
        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
           
            var token = bearerToken.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var resp = await _client.SendAsync(request);
        return resp.IsSuccessStatusCode;
    }
}
