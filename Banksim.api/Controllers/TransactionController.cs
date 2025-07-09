using BankSim.Application.DTOs;
using BankSim.Application.Services;
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

      
        [HttpPost]
        public async Task<IActionResult> Transfer([FromBody] TransactionDto dto)
        {
            await _transactionService.TransferAsync(dto);
            return Ok("Para transferi başarılı.");
        }

       
        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetByAccount(int accountId)
        {
            var transactions = await _transactionService.GetByAccountIdAsync(accountId);
            return Ok(transactions);
        }

   
        [HttpGet("account/{accountId}/summary")]
        public async Task<IActionResult> GetMiniStatement(int accountId)
        {
            var list = await _transactionService.GetByAccountIdAsync(accountId);
            var sorted = list
                .OrderByDescending(t => t.TransactionDate)
                .Take(10);

            return Ok(sorted);
        }

    }
}
