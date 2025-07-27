using BankSim.Ui.Models;

namespace BankSim.Ui.Services
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetAccountsByCustomerIdAsync(int customerId);
        Task<AccountDto?> GetAccountByIdAsync(int id);
        Task CreateAccountAsync(CreateAccountDto dto);
    }
}
