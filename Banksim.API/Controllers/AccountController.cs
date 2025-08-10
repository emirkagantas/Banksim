using BankSim.Application.DTOs;
using BankSim.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize]
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var accounts = await _accountService.GetByCustomerIdAsync(customerId);
            return Ok(accounts);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
                return NotFound(new { message = "Hesap bulunamadı." });
            return Ok(account);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
        {
            await _accountService.CreateAsync(dto);
            return Ok(new { message = "Hesap başarıyla oluşturuldu." });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _accountService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(int id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
                return NotFound(new { message = "Hesap bulunamadı." });
            return Ok(new { account.IBAN, account.Balance });
        }

        [Authorize]
        [HttpGet("iban/{iban}")]
        public async Task<IActionResult> GetByIban(string iban)
        {
            var account = await _accountService.GetByIbanAsync(iban);
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        [Authorize]
        [HttpPost("deduct")]
        public async Task<IActionResult> DeductBalance([FromBody] DeductBalanceRequest dto)
        {
            var (success, error) = await _accountService.DeductBalanceAsync(dto.AccountId, dto.Amount);
            if (!success)
                return BadRequest(error);
            return Ok("Bakiye başarıyla düşüldü.");
        }


    }
}
