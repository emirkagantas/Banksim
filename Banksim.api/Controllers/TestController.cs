using BankSim.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankSim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly BankSimDbContext _dbContext;
        public TestController(BankSimDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("hello")]
        public IActionResult Hello()
        {
            
            return Ok("Merhaba BankSim!");
        }
        [HttpPost("admin/reset-db")]
        public async Task<IActionResult> ResetDb()
        {
            // Sadece Development ortamı için!
            await _dbContext.Transactions.ExecuteDeleteAsync();
            await _dbContext.Accounts.ExecuteDeleteAsync();
            await _dbContext.Customers.ExecuteDeleteAsync();
            return Ok("Tüm ana tablolar sıfırlandı.");
        }

    }
}
