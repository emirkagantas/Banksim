using BankSim.Ui.Models;
using BankSim.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace BankSim.Ui.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IApiService _api;

        public AccountController(IAccountService accountService, IApiService apiService)
        {
            _accountService = accountService;
            _api = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            // Kullanıcı id'sini claims'ten çek
            var customerIdClaim = User.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.NameIdentifier || x.Type == "nameid"
            );
            int customerId = 0;
            if (customerIdClaim != null)
                int.TryParse(customerIdClaim.Value, out customerId);

            // API'den hesapları çek
            var response = await _api.GetAsync($"/api/account/customer/{customerId}");
            var hesaplar = new List<AccountDto>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                hesaplar = JsonConvert.DeserializeObject<List<AccountDto>>(json);
            }

            ViewBag.Accounts = hesaplar;
            ViewBag.AccountCount = hesaplar.Count;
            ViewBag.Username = User.Identity.Name; // Kullanıcı adını view'da kullanabilirsin
            return View();
        }
        
        public async Task<IActionResult> Details(int id, int page = 1, string? startDate = null, string? endDate = null)
        {
            int pageSize = 10;
            List<TransactionDto> transactions;

            // ——— Tarihleri al
            DateTime start = DateTime.TryParse(startDate, out var sdt) ? sdt : DateTime.Today.AddMonths(-1);
            DateTime end = DateTime.TryParse(endDate, out var edt) ? edt : DateTime.Today.AddDays(1);

            // ——— API'ya POST ile filtreli çek
            var filter = new
            {
                accountId = id,
                startDate = start.ToUniversalTime().ToString("o"),
                endDate = end.ToUniversalTime().ToString("o")
            };

            var response = await _api.PostAsync("/api/transaction/filter", filter);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                transactions = JsonConvert.DeserializeObject<List<TransactionDto>>(json) ?? new List<TransactionDto>();
            }
            else
            {
                transactions = new List<TransactionDto>();
            }

            // Sıralama & Sayfalama
            var sorted = transactions.OrderByDescending(x => x.TransactionDate).ToList();
            int totalCount = sorted.Count;
            var paged = sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Hesap bilgisi
            AccountDto account = null!;
            var accRes = await _api.GetAsync($"/api/account/{id}");
            if (accRes.IsSuccessStatusCode)
            {
                var json = await accRes.Content.ReadAsStringAsync();
                account = JsonConvert.DeserializeObject<AccountDto>(json)!;
            }

            // ViewBag’lere tarihleri ve string karşılıklarını da at
            ViewBag.Account = account;
            ViewBag.Transfers = paged;
            ViewBag.TotalCount = totalCount;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.AccountId = id;
            ViewBag.StartDateString = start.ToString("yyyy-MM-dd");
            ViewBag.EndDateString = end.ToString("yyyy-MM-dd");
            ViewBag.DefaultStartDate = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.DefaultEndDate = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.Username = User.Identity.Name;

            return View();
        }




        [HttpGet]
        public IActionResult Create()
        {
         
            var currencyOptions = new List<SelectListItem>
    {
            new SelectListItem { Value = "1", Text = "TL" },
            new SelectListItem { Value = "2", Text = "USD" }
       
    };
            ViewBag.CurrencyOptions = currencyOptions;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

           
            var customerIdClaim = User.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.NameIdentifier || x.Type == "nameid"
            );
            if (customerIdClaim != null)
                model.CustomerId = int.Parse(customerIdClaim.Value);

            await _accountService.CreateAccountAsync(model);
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> ExportExcel(int id, string? startDate = null, string? endDate = null)
        {
            // Filtreleme ile aynı şekilde: Anonim filter objesi hazırla
            var filter = new
            {
                accountId = id,
                startDate = startDate,
                endDate = endDate
            };

            // API'ya filter'ı olduğu gibi gönder
            var response = await _api.PostAsync("/api/transaction/export-excel", filter);

            if (!response.IsSuccessStatusCode)
                return Content("Excel aktarımı başarısız!");

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var contentDisposition = response.Content.Headers.ContentDisposition;
            var fileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName ?? $"transferler_{id}.xlsx";
            var contentType = response.Content.Headers.ContentType?.MediaType
                              ?? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(bytes, contentType, fileName);
        }



        [HttpPost]
        public async Task<IActionResult> ExportPdf(int id, string? startDate = null, string? endDate = null)
        {
            var filter = new
            {
                accountId = id,
                startDate = startDate,
                endDate = endDate
            };

            var response = await _api.PostAsync("/api/transaction/export-pdf", filter);

            if (!response.IsSuccessStatusCode)
                return Content("PDF aktarımı başarısız!");

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var contentDisposition = response.Content.Headers.ContentDisposition;
            var fileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName ?? $"transferler_{id}.pdf";
            var contentType = response.Content.Headers.ContentType?.MediaType
                              ?? "application/pdf";

            return File(bytes, contentType, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Transfer()
        {
            // Kullanıcı id'sini claims'ten çek
            var customerIdClaim = User.Claims.FirstOrDefault(x =>
                x.Type == System.Security.Claims.ClaimTypes.NameIdentifier || x.Type == "nameid");
            int customerId = 0;
            if (customerIdClaim != null)
                int.TryParse(customerIdClaim.Value, out customerId);

            // API'dan hesapları çek (UI service değil, API çağrısı!)
            var response = await _api.GetAsync($"/api/account/customer/{customerId}");
            var hesaplar = new List<AccountDto>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                hesaplar = JsonConvert.DeserializeObject<List<AccountDto>>(json);
            }

            ViewBag.Accounts = hesaplar;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Transfer(int FromAccountId, string ToIban, decimal Amount)
        {
            var toAccountResponse = await _api.GetAsync($"/api/account/iban/{ToIban}");
            if (!toAccountResponse.IsSuccessStatusCode)
            {
                TempData["TransferError"] = "Alıcı IBAN bulunamadı!";
                return RedirectToAction("Index", "Dashboard");
            }

            var toAccountJson = await toAccountResponse.Content.ReadAsStringAsync();
            var toAccount = JsonConvert.DeserializeObject<AccountDto>(toAccountJson);
            if (toAccount == null)
            {
                TempData["TransferError"] = "Alıcı IBAN bulunamadı!";
                return RedirectToAction("Index", "Dashboard");
            }

            var transferDto = new
            {
                FromAccountId = FromAccountId,
                ToAccountId = toAccount.Id,
                Amount = Amount
            };

            var response = await _api.PostAsync("/api/Transaction", transferDto);
            if (!response.IsSuccessStatusCode)
            {
                TempData["TransferError"] = "Transfer başarısız!";
            }
            else
            {
                TempData["TransferSuccess"] = "Transfer başarıyla gerçekleştirildi!";
            }

            return RedirectToAction("Index", "Dashboard");
        }


    }
}

