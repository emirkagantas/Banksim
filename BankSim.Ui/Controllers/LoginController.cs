using BankSim.Ui.Controllers;
using BankSim.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[AllowAnonymous]
public class LoginController : BaseController
{
    private readonly IApiService _api;

    public LoginController(IApiService api)
    {
        _api = api;
    }

    public IActionResult Index()
    {
        if (Request.Cookies["token"] != null)
            return RedirectToAction("Index", "Dashboard");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string email, string password)
    {
        var response = await _api.PostAsync("/api/auth/login", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Hata = "Giriş başarısız!";
            return View();
        }

        var json = await response.Content.ReadAsStringAsync();
        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        if (dict == null || !dict.ContainsKey("token"))
        {
            ViewBag.Hata = "Giriş başarısız! (token yok)";
            return View();
        }
        var token = dict["token"];
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

        DateTimeOffset expires = DateTimeOffset.UtcNow.AddHours(1); 
        if (long.TryParse(expClaim, out var unixExp))
            expires = DateTimeOffset.FromUnixTimeSeconds(unixExp);


        Response.Cookies.Append("token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure =  true,
            SameSite = SameSiteMode.None,
            Expires = expires.UtcDateTime
        });

        return RedirectToAction("Index", "Dashboard");
    }
}
