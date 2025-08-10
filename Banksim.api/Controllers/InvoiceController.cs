using BankSim.Application.DTOs;
using BankSim.Application.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpGet("customer/{tckn}")]
        public async Task<IActionResult> GetInvoices(string tckn)
        {
            var invoices = await _invoiceService.GetInvoicesAsync(tckn);
            return Ok(invoices);
        }
       
        [Authorize]
        [HttpPost("pay")]
        public async Task<IActionResult> PayInvoice([FromBody] PayInvoiceRequest dto)
        {
            var bearer = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(bearer) || !bearer.StartsWith("Bearer "))
                return Unauthorized("Bearer token eksik.");

            var token = bearer.Substring("Bearer ".Length).Trim();

            var result = await _invoiceService.PayInvoiceAsync(dto, token);
            if (!result) return BadRequest("Ödeme başarısız.");
            return Ok("Fatura ödendi.");
        }

    }

}
