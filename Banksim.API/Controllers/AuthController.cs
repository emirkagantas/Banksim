using BankSim.Application.DTOs;
using BankSim.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankSim.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICustomerRepository _customerRepository;

        public AuthController(
            IAuthService authService,
            ICustomerRepository customerRepository)
        {
            _authService = authService;
            _customerRepository = customerRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            
            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Şifreler uyuşmuyor.");

            await _authService.RegisterAsync(dto);
            return Ok(new { message = "Kayıt başarılı. Şimdi giriş yapabilirsiniz!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
                return Unauthorized(new { message = "Geçersiz e-posta veya şifre." });

            return Ok(new { token });
        }

        
    }
}
