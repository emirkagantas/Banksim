using BankSim.Domain.Entities;

namespace BankSim.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllByCustomerIdAsync(int customerId);
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> GetByIbanAsync(string iban);
        Task AddAsync(Account account);
        void Delete(Account account);
        Task SaveChangesAsync();

    }
}
