using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankSim.Ui.Models;
using BankSim.Ui.Services;

namespace BankSim.Ui.Controllers
{
    [AllowAnonymous]
    public class RegisterController : BaseController
    {
        private readonly IApiService _api;

        public RegisterController(IApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new RegisterDto());
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegisterDto model)
        {
            var response = await _api.PostAsync("/api/auth/register", model);

            if (!response.IsSuccessStatusCode)
            {
          
                var errorMsg = await _api.GetErrorMessageAsync(response);
                ViewBag.Error = errorMsg ?? "Beklenmeyen bir hata oluştu.";
                return View(model);
            }

            return RedirectToAction("Index", "Login");
        }
    }
}
