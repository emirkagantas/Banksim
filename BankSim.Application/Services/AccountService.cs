using AutoMapper;
using BankSim.Application.DTOs;
using BankSim.Application.Utils;
using BankSim.Domain.Entities;
using BankSim.Domain.Enums;
using BankSim.Domain.Interfaces;

namespace BankSim.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IAccountRepository repository, ICustomerRepository customerRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

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

        public async Task<AccountDto?> GetByIbanAsync(string iban)
        {
            var account = await _repository.GetByIbanAsync(iban);
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
                Balance = 0,
                Currency = (Currency)dto.Currency
            };

            await _repository.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();



        }
        public async Task DeleteAsync(int id)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null) return;

            _repository.Delete(account);
            await _unitOfWork.SaveChangesAsync();

        }
     
        
        public async Task<string> GetAccountOwnerNameAsync(int accountId)
        {
            var account = await _repository.GetByIdAsync(accountId);
            return account?.Customer?.FullName ?? "kullanici";
        }



        public async Task<(bool Success, string ErrorMessage)> DeductBalanceAsync(int accountId, decimal amount)
        {
            var account = await _repository.GetByIdAsync(accountId);
            if (account == null)
                return (false, "Hesap bulunamadı.");

            if (amount <= 0)
                return (false, "Tutar pozitif olmalı.");

            if (account.Balance < amount)
                return (false, "Yetersiz bakiye.");

            account.Balance -= amount;
            _repository.Update(account); 

            await _unitOfWork.SaveChangesAsync(); 
            return (true, "");
        }





    }
}

