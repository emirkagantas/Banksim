using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BankSim.Application.DTOs;

namespace BankSim.Application.Services
{
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

        public async Task<bool> PayInvoiceAsync(PayInvoiceRequest req)
        {
            var resp = await _client.PostAsJsonAsync("/invoices/pay", req);
            return resp.IsSuccessStatusCode;
        }
    }

}
