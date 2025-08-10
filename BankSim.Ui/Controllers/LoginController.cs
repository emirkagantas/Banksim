using BankSim.Ui.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
public class LoginController : BaseController
{
    public LoginController(IConfiguration configuration) : base(configuration) { }

    public IActionResult Index()
    {
        return View();
    }
}
