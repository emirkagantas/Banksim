using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSim.Application.DTOs;

namespace BankSim.Application.Services
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetByCustomerIdAsync(int customerId);
        Task<AccountDto?> GetByIdAsync(int id);
        Task CreateAsync(CreateAccountDto dto);
        Task DeleteAsync(int id);
        
        Task<string> GetAccountOwnerNameAsync(int accountId);
        Task<AccountDto?> GetByIbanAsync(string iban);

        Task<(bool Success, string ErrorMessage)> DeductBalanceAsync(int accountId, decimal amount);

    }
}

