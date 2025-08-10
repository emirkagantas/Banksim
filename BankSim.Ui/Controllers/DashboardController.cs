using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BankSim.Ui.Controllers
{
    public class DashboardController : BaseController
    {
        public DashboardController(IConfiguration configuration) : base(configuration) { }

        public IActionResult Index()
        {
          
            return View();
        }
    }
}
