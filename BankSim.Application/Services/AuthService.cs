using BankSim.Application.DTOs;
using BankSim.Application.Utils;
using BankSim.Domain.Entities;
using BankSim.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankSim.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IRedisCacheService _cacheService;

        public AuthService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IConfiguration config, IRedisCacheService cacheService)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _config = config;
            _cacheService = cacheService;

        }

        public async Task RegisterAsync(RegisterDto dto)
        {
           
            var emailExists = (await _customerRepository.GetAllAsync()).Any(x => x.Email == dto.Email);
            if (emailExists)
                throw new Exception("Bu e-posta adresi zaten kayıtlı.");

            
            var identityExists = (await _customerRepository.GetAllAsync()).Any(x => x.IdentityNumber == dto.IdentityNumber);
            if (identityExists)
                throw new Exception("Bu TC kimlik numarası ile zaten kayıt var.");

            var phoneExists =(await _customerRepository.GetAllAsync()).Any(x=> x.Phone == dto.Phone);
            if(phoneExists)
                throw new Exception("Bu Telefon numarası zaten kayıtlı.");

           
            var customer = new Customer
            {
                FullName = dto.FullName,
                IdentityNumber = dto.IdentityNumber,
                Email = dto.Email,
                Phone = dto.Phone,
                Password = PasswordHasher.HashPassword(dto.Password)
            };

            await _customerRepository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            var dtoToCache = new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone
            };
            string cacheKey = $"customer:{customer.Id}";
            await _cacheService.SetAsync(cacheKey, dtoToCache, TimeSpan.FromMinutes(15));
        }


        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var hashed = PasswordHasher.HashPassword(dto.Password);
            var user = (await _customerRepository.GetAllAsync())
                        .FirstOrDefault(x => x.Email == dto.Email && x.Password == hashed);

            if (user == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
