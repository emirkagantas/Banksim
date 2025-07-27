using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankSim.Ui.Controllers
{
    public class BaseController : Controller
    {
       
        protected string? CurrentUserName => User?.Identity?.Name;

    
        protected int CurrentUserId
        {
            get
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var id))
                    return id;
                return 0;
            }
        }
    }
}
