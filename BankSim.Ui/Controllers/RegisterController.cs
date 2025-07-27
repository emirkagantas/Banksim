using BankSim.Ui.Controllers;
using BankSim.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankSim.Ui.Models;

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
        if (Request.Cookies["token"] != null)
            return RedirectToAction("Index", "Account");
        return View(new RegisterDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RegisterDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await _api.PostAsync("/api/auth/register", model);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await _api.GetErrorMessageAsync(response);
            ViewBag.Error = errorMsg ?? "Beklenmeyen bir hata oluştu.";
            return View(model);
        }

        // Kayıt başarılı, Login sayfasına yönlendir
        return RedirectToAction("Index", "Login");
    }
}
