using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Ui.Controllers
{
    [Authorize]
    public class LogoutController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index()
        {
            // Çıkış için cookie silinir.
            var cookieOptions = new CookieOptions
            {

                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Delete("token", cookieOptions);

            // İstersen çıkış mesajı için TempData["LogoutMsg"] kullanabilirsin.

            // Login sayfasına yönlendir.
            return RedirectToAction("Index", "Login");
        }
    }
}
