using BankSim.Ui.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class AccountController : BaseController
{
    public AccountController(IConfiguration configuration) : base(configuration) { }

   
    public IActionResult Index()
    {
        return View(); 
    }

    
    public IActionResult Details()
    {
        return View(); 
    }

   
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.CurrencyOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "TL" },
            new SelectListItem { Value = "2", Text = "USD" }
        };
        return View();
    }

  
    [HttpGet]
    public IActionResult Transfer()
    {
        return View();
    }
}
