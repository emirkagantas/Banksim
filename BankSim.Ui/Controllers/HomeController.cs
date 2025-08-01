using BankSim.Ui.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BankSim.Ui.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
       
        public HomeController() { }

       
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
