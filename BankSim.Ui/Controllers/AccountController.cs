using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BankSim.Ui.Services;
using BankSim.Ui.Models;

namespace BankSim.Ui.Controllers
{
    public class AccountController : BaseController 
    {
        private readonly ApiService _api;

        public AccountController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {

            int customerId = GetCustomerIdFromToken();

            var response = await _api.GetAsync($"https://localhost:7291/api/account/customer/{customerId}");

            if (!response.IsSuccessStatusCode)
                return View(new List<AccountDto>());

            var json = await response.Content.ReadAsStringAsync();
            var hesaplar = JsonConvert.DeserializeObject<List<AccountDto>>(json);

            return View(hesaplar);
        }
    }
}
