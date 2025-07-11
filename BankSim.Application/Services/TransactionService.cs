using AutoMapper;
using BankSim.Application.DTOs;
using BankSim.Application.Utils;
using BankSim.Domain.Entities;
using BankSim.Domain.Enums.BankSim.Domain.Enums;
using BankSim.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ExchangeRateService _rateService;

        public TransactionService(
            ITransactionRepository transactionRepo,
            IAccountRepository accountRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ExchangeRateService rateService)
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _rateService = rateService;
        }

        public async Task TransferAsync(TransactionDto dto)
        {
            if (dto.Amount <= 0) throw new Exception("Tutar sıfırdan büyük olmalı.");

            var fromAccount = await _accountRepo.GetByIdAsync(dto.FromAccountId)
                ?? throw new Exception("Gönderen hesap bulunamadı.");
            var toAccount = await _accountRepo.GetByIdAsync(dto.ToAccountId)
                ?? throw new Exception("Alıcı hesap bulunamadı.");

            if (fromAccount.Balance < dto.Amount)
                throw new Exception("Bakiye yetersiz.");

            string fromCurrency = GetCurrencyCode(fromAccount.Currency);
            string toCurrency = GetCurrencyCode(toAccount.Currency);

            decimal rate = 1m;
            if (fromCurrency != toCurrency)
                rate = await _rateService.GetExchangeRateAsync(fromCurrency, toCurrency);

            decimal convertedAmount = dto.Amount * rate;

            fromAccount.Balance -= dto.Amount;
            toAccount.Balance += convertedAmount;

            var transaction = new Transaction
            {
                FromAccountId = dto.FromAccountId,
                ToAccountId = dto.ToAccountId,
                Amount = dto.Amount,
                TransactionDate = DateTime.UtcNow,
                ExchangeRate = fromCurrency != toCurrency ? rate : null,
                ConvertedAmount = fromCurrency != toCurrency ? convertedAmount : null
            };

            await _transactionRepo.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        private string GetCurrencyCode(Currency currency)
        {
            return currency == Currency.TL ? "TRY" : "USD";
        }


        public async Task<List<TransactionDto>> GetByAccountIdAsync(int accountId)
        {
            var list = await _transactionRepo.GetByAccountIdAsync(accountId);
            return _mapper.Map<List<TransactionDto>>(list);
        }

        public async Task<List<TransactionDto>> GetByFilterAsync(TransactionFilterDto filter)
        {
            var all = await _transactionRepo.GetByAccountIdAsync(filter.AccountId);
            var filtered = all
                .Where(t => t.TransactionDate >= filter.StartDate && t.TransactionDate <= filter.EndDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return _mapper.Map<List<TransactionDto>>(filtered);
        }

        public async Task<List<TransactionExportDto>> GetExportListAsync(TransactionFilterDto filter)
        {
            var filtered = await _transactionRepo.GetByAccountIdAsync(filter.AccountId);
            filtered = filtered
                .Where(t => t.TransactionDate >= filter.StartDate && t.TransactionDate <= filter.EndDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            var accountIds = filtered
                .SelectMany(t => new[] { t.FromAccountId, t.ToAccountId })
                .Distinct()
                .ToList();

            var accounts = await _accountRepo.GetByIdsAsync(accountIds);
            var accountsDict = accounts.ToDictionary(a => a.Id);

            var exportList = new List<TransactionExportDto>();
            foreach (var t in filtered)
            {
                var fromAccount = accountsDict.GetValueOrDefault(t.FromAccountId);
                var toAccount = accountsDict.GetValueOrDefault(t.ToAccountId);

                exportList.Add(new TransactionExportDto
                {
                    FromIban = fromAccount?.IBAN ?? "",
                    FromFullName = fromAccount?.Customer?.FullName ?? "",
                    ToIban = toAccount?.IBAN ?? "",
                    ToFullName = toAccount?.Customer?.FullName ?? "",
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    ExchangeRate = t.ExchangeRate,
                    ConvertedAmount = t.ConvertedAmount
                });
            }
            return exportList;
        }



    }
}
