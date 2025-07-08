using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSim.Domain.Entities;
using BankSim.Domain.Interfaces;
using BankSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankSim.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankSimDbContext _context;

        public TransactionRepository(BankSimDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetByAccountIdAsync(int accountId)
        {
            return await _context.Transactions
                .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
                .ToListAsync();
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }
    }
}

