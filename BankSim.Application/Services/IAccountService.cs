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
        Task<bool> IsAccountBelongsToUser(int accountId, string userEmail);
        Task<string> GetAccountOwnerNameAsync(int accountId);
        Task<AccountDto?> GetByIbanAsync(string iban);

    }
}

