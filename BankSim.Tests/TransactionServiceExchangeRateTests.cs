using Xunit;
using Moq;
using AutoMapper;
using BankSim.Application.Services;
using BankSim.Application.DTOs;
using BankSim.Domain.Interfaces;
using BankSim.Domain.Entities;
using BankSim.Domain.Enums;

namespace BankSim.Tests
{
    public class TransactionServiceTests
    {
        private TransactionService CreateService(
            out Mock<ITransactionRepository> transRepo,
            out Mock<IAccountRepository> accRepo,
            out Mock<IUnitOfWork> uow,
            out Mock<IExchangeRateService> exchangeService,
            out IMapper mapper)
        {
            transRepo = new Mock<ITransactionRepository>();
            accRepo = new Mock<IAccountRepository>();
            uow = new Mock<IUnitOfWork>();
            exchangeService = new Mock<IExchangeRateService>();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<BankSim.Application.Mapping.CustomerProfile>());
            mapper = mapperConfig.CreateMapper();

            return new TransactionService(
                transRepo.Object, accRepo.Object, mapper, uow.Object, exchangeService.Object);
        }

        [Fact]
        public async Task TransferAsync_UsesExchangeRateService_WhenCurrencyDifferent()
        {
            var service = CreateService(out var transRepo, out var accRepo, out var uow, out var exchangeService, out var mapper);

            var fromAccount = new Account { Id = 1, Balance = 100, Currency = Currency.TL };
            var toAccount = new Account { Id = 2, Balance = 0, Currency = Currency.USD };

            accRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => id == 1 ? fromAccount : toAccount);

            exchangeService
                .Setup(x => x.GetExchangeRateAsync("TRY", "USD"))
                .ReturnsAsync(0.03m);

            var dto = new TransactionDto
            {
                FromAccountId = 1,
                ToAccountId = 2,
                Amount = 50
            };

            await service.TransferAsync(dto);

            Assert.Equal(50, fromAccount.Balance);
            Assert.Equal(1.5m, toAccount.Balance);
            exchangeService.Verify(x => x.GetExchangeRateAsync("TRY", "USD"), Times.Once);
            Assert.True(Object.ReferenceEquals(toAccount, (await accRepo.Object.GetByIdAsync(2))));
        }

        [Fact]
        public async Task TransferAsync_ThrowsException_WhenInsufficientBalance()
        {
            var service = CreateService(out var transRepo, out var accRepo, out var uow, out var exchangeService, out var mapper);

            var fromAccount = new Account { Id = 1, Balance = 10, Currency = Currency.TL };
            var toAccount = new Account { Id = 2, Balance = 0, Currency = Currency.TL };

            accRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => id == 1 ? fromAccount : toAccount);

            var dto = new TransactionDto
            {
                FromAccountId = 1,
                ToAccountId = 2,
                Amount = 50
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.TransferAsync(dto));
            Assert.Contains("Bakiye yetersiz", ex.Message);
        }

        [Fact]
        public async Task TransferAsync_ThrowsException_WhenAmountIsNegative()
        {
            var service = CreateService(out var transRepo, out var accRepo, out var uow, out var exchangeService, out var mapper);

            var fromAccount = new Account { Id = 1, Balance = 100, Currency = Currency.TL };
            var toAccount = new Account { Id = 2, Balance = 0, Currency = Currency.TL };

            accRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => id == 1 ? fromAccount : toAccount);

            var dto = new TransactionDto
            {
                FromAccountId = 1,
                ToAccountId = 2,
                Amount = -20
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.TransferAsync(dto));
            Assert.Contains("Tutar sıfırdan büyük olmalı", ex.Message);
        }

        [Fact]
        public async Task TransferAsync_DoesNotCallExchangeService_WhenCurrencySame()
        {
            var service = CreateService(out var transRepo, out var accRepo, out var uow, out var exchangeService, out var mapper);

            var fromAccount = new Account { Id = 1, Balance = 100, Currency = Currency.TL };
            var toAccount = new Account { Id = 2, Balance = 0, Currency = Currency.TL };

            accRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => id == 1 ? fromAccount : toAccount);

            var dto = new TransactionDto
            {
                FromAccountId = 1,
                ToAccountId = 2,
                Amount = 30
            };

            await service.TransferAsync(dto);

            exchangeService.Verify(x => x.GetExchangeRateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.Equal(70, fromAccount.Balance);
            Assert.Equal(30, toAccount.Balance);
        }

        [Fact]
        public async Task TransferAsync_ThrowsException_WhenAccountNotFound()
        {
            var service = CreateService(out var transRepo, out var accRepo, out var uow, out var exchangeService, out var mapper);

            accRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Account)null!);

            var dto = new TransactionDto
            {
                FromAccountId = 99,
                ToAccountId = 100,
                Amount = 10
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.TransferAsync(dto));
            Assert.Contains("Gönderen hesap bulunamadı", ex.Message);
        }
    }
}
