using BankSim.Ui.Models;

namespace BankSim.Ui.Services 
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetAccountsByUserAsync(string userName);
        Task<AccountDto> GetAccountByIdAsync(int id);
        Task CreateAccountAsync(string userName, CreateAccountDto dto);
    }
}