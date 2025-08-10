using BankSim.Ui.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
public class RegisterController : BaseController
{
    public RegisterController(IConfiguration configuration) : base(configuration) { }

    [HttpGet]
    public IActionResult Index()
    {
       
        if (false)
            return RedirectToAction("Index", "Dashboard");
        return View();
    }
}
