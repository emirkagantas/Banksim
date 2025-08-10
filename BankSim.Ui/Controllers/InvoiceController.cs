using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BankSim.Ui.Controllers
{
    public class InvoiceController : Controller
    {
        public InvoiceController() { }

        [HttpGet]
        public IActionResult Index()
        {
         
            return View();
        }
    }
}
