using BankSim.Ui.Models;
using BankSim.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Ui.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
   

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accounts = await _accountService.GetAccountsByUserAsync(User.Identity.Name);
            return View(accounts);
        }

      
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound();
            return View(account);
        }

            [HttpGet]
            public IActionResult Create()
            {
                return View();
            }


            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(CreateAccountDto model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                await _accountService.CreateAccountAsync(User.Identity.Name, model);
                return RedirectToAction(nameof(Index));
            }

        }

    } 

