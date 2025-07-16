using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankSim.Ui.Controllers
{
    public class BaseController : Controller
    {
       
        public string? GetToken()
        {
            return HttpContext.Session.GetString("token");
        }

       
        public int GetCustomerIdFromToken()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
                return 0;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid");
            return idClaim != null ? int.Parse(idClaim.Value) : 0;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = HttpContext.Session.GetString("token");
           
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            bool allowAnonymous =
            (actionDescriptor?.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any() ?? false)
            ||
            (actionDescriptor?.ControllerTypeInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any() ?? false);


            if (string.IsNullOrEmpty(token) && !allowAnonymous)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }

            base.OnActionExecuting(context);
        }

    }
}
