using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BankSim.Application.DTOs;
using BankSim.Application.Utils;
using BankSim.Domain.Entities;
using BankSim.Domain.Interfaces;

namespace BankSim.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository repository, ICustomerRepository customerRepository, IMapper mapper)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<List<AccountDto>> GetByCustomerIdAsync(int customerId)
        {
            var accounts = await _repository.GetAllByCustomerIdAsync(customerId);
            return _mapper.Map<List<AccountDto>>(accounts);
        }

        public async Task<AccountDto?> GetByIdAsync(int id)
        {
            var account = await _repository.GetByIdAsync(id);
            return _mapper.Map<AccountDto?>(account);
        }

        public async Task CreateAsync(CreateAccountDto dto)
        {
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null) throw new Exception("Müşteri bulunamadı.");

            var account = new Account
            {
                CustomerId = dto.CustomerId,
                IBAN = IbanGenerator.Generate(),
                Balance = 0
            };

            await _repository.AddAsync(account);
            await _repository.SaveChangesAsync();

        }

        public async Task DeleteAsync(int id)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null) return;

            _repository.Delete(account);
            await _repository.SaveChangesAsync();
        }
    }
}

