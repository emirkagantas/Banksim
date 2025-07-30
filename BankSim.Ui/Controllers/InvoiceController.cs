using BankSim.Ui.Models;
using BankSim.Ui.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Ui.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> List(string tckn)
        {
            var invoices = await _invoiceService.GetInvoicesByTcknAsync(tckn);
            return View("List", invoices);
        }

        [HttpPost]
        public async Task<IActionResult> Pay(int invoiceId, string tckn)
        {
            try
            {
                await _invoiceService.PayInvoiceAsync(invoiceId, tckn);
                TempData["Message"] = "Fatura başarıyla ödendi.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
