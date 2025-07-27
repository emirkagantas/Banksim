using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankSim.Ui.Services;
using BankSim.Ui.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BankSim.Ui.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IApiService _api;

        public DashboardController(IApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            int? customerId = CurrentUserId; 
            if (customerId == 0)
                return RedirectToAction("Logout", "Auth"); 

            // Müşteri bilgisi
            CustomerDto? user = null;
            var userRes = await _api.GetAsync($"/api/customer/{customerId}");
            if (userRes.IsSuccessStatusCode)
            {
                var userJson = await userRes.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<CustomerDto>(userJson);
            }

            // Hesaplar
            var response = await _api.GetAsync($"/api/account/customer/{customerId}");
            var hesaplar = new List<AccountDto>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                hesaplar = JsonConvert.DeserializeObject<List<AccountDto>>(json);
            }

            ViewBag.CustomerName = user?.FullName ?? User.Identity.Name ?? "Kullanıcı";
            ViewBag.Accounts = hesaplar.Take(2).ToList();
            ViewBag.AllAccounts = hesaplar.Count;

            return View();
        }

    }
}
