using BankSim.Application.DTOs;
using BankSim.Infrastructure.Persistence;
using BankSim.Domain.Entities;
using BankSim.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace BankSim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BankSimDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(BankSimDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (_context.Customers.Any(x => x.Email == dto.Email))
                return BadRequest("Bu email zaten kayıtlı.");

            var customer = new Customer
            {
                FullName = dto.FullName,
                IdentityNumber = dto.IdentityNumber,
                Email = dto.Email,
                Phone = dto.Phone,
                Password = PasswordHasher.HashPassword(dto.Password)
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return Ok("Kayıt başarılı.");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var hashedPassword = PasswordHasher.HashPassword(dto.Password);

            var user = _context.Customers
                .FirstOrDefault(x => x.Email == dto.Email && x.Password == hashedPassword);

            if (user == null)
                return Unauthorized("Hatalı giriş.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}
