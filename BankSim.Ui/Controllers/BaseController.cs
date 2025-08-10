using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace BankSim.Ui.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration _configuration;

        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.ApiBaseUrl = _configuration["ApiBaseUrl"];
            base.OnActionExecuting(context);
        }
    }
}
