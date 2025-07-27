using BankSim.Application.DTOs;
using BankSim.Application.Services;
using BankSim.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BankSim.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;

        public TransactionController(ITransactionService transactionService, IAccountService accountService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Transfer([FromBody] TransactionDto dto)
        {
            await _transactionService.TransferAsync(dto);
            return Ok(new { message = "Para transferi başarılı." });
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

          
          
            var customerName = await _accountService.GetAccountOwnerNameAsync(filter.accountId);
            var fileName = FileNameHelper.BuildExportFileName(customerName, "xlsx");


        

            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }




        [Authorize]
        [HttpPost("export-pdf")]
        public async Task<IActionResult> ExportToPdf([FromBody] TransactionFilterDto filter)
        {
            var exportList = await _transactionService.GetExportListAsync(filter);
            var fileContent = PdfExportHelper.ExportTransactions(exportList);

            if (fileContent == null || fileContent.Length == 0)
                return StatusCode(500, new { message = "PDF içeriği boş döndü." });

            // Doğru dosya ismi için müşteri adını AccountService'den çek!
            var customerName = await _accountService.GetAccountOwnerNameAsync(filter.accountId);
            var fileName = FileNameHelper.BuildExportFileName(customerName, "pdf");

            return File(fileContent, "application/pdf", fileName);
        }

    }
}
