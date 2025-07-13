using Microsoft.AspNetCore.Mvc;
using BankSim.Ui.Services;
using BankSim.Ui.Models;
using Newtonsoft.Json;

namespace BankSim.Ui.Controllers
{
    public class TransactionController : BaseController 
    {
        private readonly ApiService _api;

        public TransactionController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {

            int customerId = GetCustomerIdFromToken();

            var response = await _api.GetAsync($"https://localhost:7291/api/account/customer/{customerId}");
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
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Login");
            int customerId = GetCustomerIdFromToken();

            var responseAccounts = await _api.GetAsync($"https://localhost:7291/api/account/customer/{customerId}");
            var hesaplar = new List<AccountDto>();
            if (responseAccounts.IsSuccessStatusCode)
            {
                var json = await responseAccounts.Content.ReadAsStringAsync();
                hesaplar = JsonConvert.DeserializeObject<List<AccountDto>>(json);
            }
            ViewBag.Accounts = hesaplar;

            var response = await _api.PostAsync("https://localhost:7291/api/transaction", dto);

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
