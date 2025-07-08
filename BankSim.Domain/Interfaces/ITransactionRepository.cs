using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSim.Domain.Entities;

namespace BankSim.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetByAccountIdAsync(int accountId);
        Task AddAsync(Transaction transaction);
    }
}
