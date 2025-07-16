using Microsoft.AspNetCore.Mvc;
using BankSim.Ui.Services;
using BankSim.Ui.Models;
using Newtonsoft.Json;

namespace BankSim.Ui.Controllers
{
    public class TransactionController : BaseController
    {
        private readonly IApiService _api;

        public TransactionController(IApiService api)
        {
            _api = api;
        }


        public async Task<IActionResult> Index()
        {
            int customerId = GetCustomerIdFromToken();

            var response = await _api.GetAsync($"/api/account/customer/{customerId}");
            var hesaplar = new List<AccountDto>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                hesaplar = JsonConvert.DeserializeObject<List<AccountDto>>(json);
            }

            ViewBag.Accounts = hesaplar;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(TransactionDto dto)
        {
            int customerId = GetCustomerIdFromToken();

            var responseAccounts = await _api.GetAsync($"/api/account/customer/{customerId}");
            var hesaplar = new List<AccountDto>();
            if (responseAccounts.IsSuccessStatusCode)
            {
                var json = await responseAccounts.Content.ReadAsStringAsync();
                hesaplar = JsonConvert.DeserializeObject<List<AccountDto>>(json);
            }
            ViewBag.Accounts = hesaplar;

            var response = await _api.PostAsync("/api/transaction", dto);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Mesaj = "Transfer başarısız!";
                return View();
            }

            ViewBag.Mesaj = "Transfer başarılı!";
            return View();
        }
    }
}
