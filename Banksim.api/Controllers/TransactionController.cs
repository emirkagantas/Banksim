using BankSim.Application.DTOs;
using BankSim.Application.Services;
using BankSim.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Transfer([FromBody] TransactionDto dto)
        {
            await _transactionService.TransferAsync(dto);
            return Ok("Para transferi başarılı.");
        }

        [Authorize]
        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetByAccount(int accountId)
        {
            var transactions = await _transactionService.GetByAccountIdAsync(accountId);
            return Ok(transactions);
        }

        [Authorize]
        [HttpGet("account/{accountId}/summary")]
        public async Task<IActionResult> GetMiniStatement(int accountId)
        {
            var list = await _transactionService.GetByAccountIdAsync(accountId);
            var sorted = list
                .OrderByDescending(t => t.TransactionDate)
                .Take(10);

            return Ok(sorted);
        }

        [Authorize]
        [HttpPost("filter")]
        public async Task<IActionResult> GetByFilter([FromBody] TransactionFilterDto filter)
        {
            var list = await _transactionService.GetByFilterAsync(filter);
            return Ok(list);
        }

        [Authorize]
        [HttpPost("export-excel")]
        public async Task<IActionResult> ExportToExcel([FromBody] TransactionFilterDto filter)
        {
            var exportList = await _transactionService.GetExportListAsync(filter);
            var fileContent = ExcelExportHelper.ExportTransactions(exportList);
            var fileName = FileNameHelper.BuildExportFileName(exportList.FirstOrDefault()?.FromFullName, "xlsx");
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [Authorize]
        [HttpPost("export-pdf")]
        public async Task<IActionResult> ExportToPdf([FromBody] TransactionFilterDto filter)
        {
            try
            {
                var exportList = await _transactionService.GetExportListAsync(filter);
                var fileContent = PdfExportHelper.ExportTransactions(exportList);

                if (fileContent == null || fileContent.Length == 0)
                    return StatusCode(500, "PDF içeriği boş döndü.");

                var fileName = FileNameHelper.BuildExportFileName(exportList.FirstOrDefault()?.FromFullName, "pdf");
                return File(fileContent, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
               
                Console.WriteLine("PDF export hatası: " + ex.ToString());
                return StatusCode(500, "PDF oluşturulurken hata oluştu.");
            }
        }








    }
}

