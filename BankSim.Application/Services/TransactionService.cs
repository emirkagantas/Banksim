using AutoMapper;
using BankSim.Application.DTOs;
using BankSim.Domain.Entities;
using BankSim.Domain.Enums;
using BankSim.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace BankSim.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExchangeRateService _exchangeService;

        public TransactionService(
            ITransactionRepository transactionRepo,
            IAccountRepository accountRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IExchangeRateService exchangeService)
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _exchangeService = exchangeService;
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
                rate = await _exchangeService.GetExchangeRateAsync(fromCurrency, toCurrency);

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


            var senderCustomer = fromAccount.Customer;
            var receiverCustomer = toAccount.Customer;

            if (senderCustomer == null || string.IsNullOrEmpty(senderCustomer.Email))
                throw new Exception("Gönderen kullanıcının e-posta adresi bulunamadı!");
            if (receiverCustomer == null || string.IsNullOrEmpty(receiverCustomer.Email))
                throw new Exception("Alıcı kullanıcının e-posta adresi bulunamadı!");

            var emailToSender = new EmailMessageDto
            {
                To = senderCustomer.Email,
                Subject = "Transfer Bildirimi",
                Body = $"{dto.Amount} TL tutarındaki transferiniz başarıyla gerçekleşti. Alıcı: {receiverCustomer.FullName} ({toAccount.IBAN})"
            };

            var emailToReceiver = new EmailMessageDto
            {
                To = receiverCustomer.Email,
                Subject = "Hesabınıza Para Geldi!",
                Body = $"{senderCustomer.FullName} ({fromAccount.IBAN}) hesabından {dto.Amount} TL tutarında bir transfer aldınız."
            };

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "transaction.mail",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

           
            var jsonSender = System.Text.Json.JsonSerializer.Serialize(emailToSender);
            var bodySender = System.Text.Encoding.UTF8.GetBytes(jsonSender);
            channel.BasicPublish(exchange: "",
                                 routingKey: "transaction.mail",
                                 basicProperties: null,
                                 body: bodySender);

          
            var jsonReceiver = System.Text.Json.JsonSerializer.Serialize(emailToReceiver);
            var bodyReceiver = System.Text.Encoding.UTF8.GetBytes(jsonReceiver);
            channel.BasicPublish(exchange: "",
                                 routingKey: "transaction.mail",
                                 basicProperties: null,
                                 body: bodyReceiver);
        }


        private string GetCurrencyCode(Currency currency)
        {
            return currency == Currency.TL ? "TRY" : "USD";
        }


        public async Task<List<TransactionDto>> GetByAccountIdAsync(int accountId)
        {
            var list = await _transactionRepo.GetByAccountIdAsync(accountId);

            
            var accountIds = list
                .SelectMany(t => new[] { t.FromAccountId, t.ToAccountId })
                .Distinct()
                .ToList();

            var accounts = await _accountRepo.GetByIdsAsync(accountIds); 
            var accountDict = accounts.ToDictionary(a => a.Id, a => a);

            var dtos = list.Select(t =>
            {
                var fromAccount = accountDict.GetValueOrDefault(t.FromAccountId);
                var toAccount = accountDict.GetValueOrDefault(t.ToAccountId);

                return new TransactionDto
                {
                    FromAccountId = t.FromAccountId,
                    FromAccountName = fromAccount?.Customer?.FullName ?? t.FromAccountId.ToString(),
                    ToAccountId = t.ToAccountId,
                    ToAccountName = toAccount?.Customer?.FullName ?? t.ToAccountId.ToString(),
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate
                };
            }).ToList();

            return dtos;
        }

        public async Task<List<TransactionDto>> GetByFilterAsync(TransactionFilterDto filter)
        {
        
            var all = await _transactionRepo.GetByAccountIdAsync(filter.accountId);
            Console.WriteLine($"[Filter] AccountId={filter.accountId}, total transactions: {all.Count}");

          
            DateTime? startDateOnly = filter.startDate?.Date;
            DateTime? endDateOnly = filter.endDate?.Date;

            var filtered = all
                .Where(t =>
                    (!startDateOnly.HasValue || t.TransactionDate.Date >= startDateOnly.Value) &&
                    (!endDateOnly.HasValue || t.TransactionDate.Date <= endDateOnly.Value)
                )
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

       
            var accountIds = filtered
                .SelectMany(t => new[] { t.FromAccountId, t.ToAccountId })
                .Distinct()
                .ToList();

            var accounts = await _accountRepo.GetByIdsAsync(accountIds);
            var accountDict = accounts.ToDictionary(a => a.Id, a => a);

            var dtos = filtered.Select(t =>
            {
                var fromAccount = accountDict.GetValueOrDefault(t.FromAccountId);
                var toAccount = accountDict.GetValueOrDefault(t.ToAccountId);

                return new TransactionDto
                {
                    FromAccountId = t.FromAccountId,
                    FromAccountName = fromAccount?.Customer?.FullName ?? t.FromAccountId.ToString(),
                    ToAccountId = t.ToAccountId,
                    ToAccountName = toAccount?.Customer?.FullName ?? t.ToAccountId.ToString(),
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate
                };
            }).ToList();

            return dtos;
        }





        public async Task<List<TransactionExportDto>> GetExportListAsync(TransactionFilterDto filter)
        {
            var all = await _transactionRepo.GetByAccountIdAsync(filter.accountId);

            IEnumerable<Transaction> filtered = all;

            if (filter.startDate.HasValue)
                filtered = filtered.Where(t => t.TransactionDate >= filter.startDate.Value);

            if (filter.endDate.HasValue)
                filtered = filtered.Where(t => t.TransactionDate <= filter.endDate.Value);

            filtered = filtered.OrderByDescending(t => t.TransactionDate);

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
