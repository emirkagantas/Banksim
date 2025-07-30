using BankSim.Ui.Models;
using Newtonsoft.Json;

namespace BankSim.Ui.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IApiService _apiService;

        public InvoiceService(IApiService apiService)
        {
            _apiService = apiService;
        }

       
        public async Task<List<InvoiceDto>> GetInvoicesByTcknAsync(string tckn)
        {
            var response = await _apiService.GetAsync($"/api/invoice/customer/{tckn}");
            if (!response.IsSuccessStatusCode)
                return new List<InvoiceDto>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<InvoiceDto>>(json) ?? new List<InvoiceDto>();
        }

       
        public async Task PayInvoiceAsync(int invoiceId, string tckn)
        {
            var dto = new PayInvoiceRequestDto
            {
                InvoiceId = invoiceId,
                CustomerTckn = tckn,
                PaymentMethod = "Hesap" 
            };

            var response = await _apiService.PostAsync("/api/invoice/pay", dto);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await _apiService.GetErrorMessageAsync(response);
                throw new Exception(errorMsg ?? "Fatura ödenemedi!");
            }
        }
    }
}
