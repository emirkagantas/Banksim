using BankSim.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankSim.Ui.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController 
    {
        private readonly ApiService _api;

        public LoginController(ApiService api)
        {
            _api = api;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            var response = await _api.PostAsync("https://localhost:7291/api/auth/login", new { email, password });

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Hata = "Giriş başarısız!";
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!["token"];
            HttpContext.Session.SetString("token", token);

            return RedirectToAction("Index", "Account");
        }
    }
}
