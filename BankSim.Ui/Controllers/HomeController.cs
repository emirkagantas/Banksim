using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Ui.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public HomeController(IConfiguration configuration) : base(configuration) { }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
     
            return View();
        }
    }
}
