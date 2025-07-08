using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BankSim.Application.DTOs;
using BankSim.Domain.Entities;
using BankSim.Domain.Interfaces;

namespace BankSim.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(
            ITransactionRepository transactionRepo,
            IAccountRepository accountRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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

            fromAccount.Balance -= dto.Amount;
            toAccount.Balance += dto.Amount;

            var transaction = new Transaction
            {
                FromAccountId = dto.FromAccountId,
                ToAccountId = dto.ToAccountId,
                Amount = dto.Amount,
                TransactionDate = DateTime.UtcNow
            };

            await _transactionRepo.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<TransactionDto>> GetByAccountIdAsync(int accountId)
        {
            var list = await _transactionRepo.GetByAccountIdAsync(accountId);
            return _mapper.Map<List<TransactionDto>>(list);
        }
    }
}
