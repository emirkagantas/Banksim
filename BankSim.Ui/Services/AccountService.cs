using BankSim.Ui.Models;
using Newtonsoft.Json;

namespace BankSim.Ui.Services
{
    public class AccountService : IAccountService
    {
        private readonly IApiService _apiService;

        public AccountService(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Kullanıcıya göre hesapları getir (id ile!)
        public async Task<List<AccountDto>> GetAccountsByCustomerIdAsync(int customerId)
        {
            var response = await _apiService.GetAsync($"/api/account/customer/{customerId}");
            if (!response.IsSuccessStatusCode)
                return new List<AccountDto>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<AccountDto>>(json) ?? new List<AccountDto>();
        }

        // Hesap id'sine göre getir
        public async Task<AccountDto?> GetAccountByIdAsync(int id)
        {
            var response = await _apiService.GetAsync($"/api/account/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AccountDto>(json);
        }

        // Yeni hesap oluştur
        public async Task CreateAccountAsync(CreateAccountDto dto)
        {
            var response = await _apiService.PostAsync("/api/account", dto);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await _apiService.GetErrorMessageAsync(response);
                throw new Exception(errorMsg ?? "Hesap oluşturulamadı!");
            }
        }
    }
}
