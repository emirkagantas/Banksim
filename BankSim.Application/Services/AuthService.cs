﻿using BankSim.Application.DTOs;
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

        public AuthService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var exists = (await _customerRepository.GetAllAsync()).Any(x => x.Email == dto.Email);
            if (exists)
                throw new Exception("This email is already registered.");

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
                new Claim(ClaimTypes.Email, user.Email)
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
