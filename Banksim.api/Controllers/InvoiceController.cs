using BankSim.Application.DTOs;
using BankSim.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("customer/{tckn}")]
        public async Task<IActionResult> GetInvoices(string tckn)
        {
            var invoices = await _invoiceService.GetInvoicesAsync(tckn);
            return Ok(invoices);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> PayInvoice([FromBody] PayInvoiceRequest dto)
        {
            var result = await _invoiceService.PayInvoiceAsync(dto);
            if (!result) return BadRequest("Ödeme başarısız.");
            return Ok("Fatura ödendi.");
        }
    }

}
