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
    public class AccountRepository : IAccountRepository
    {
        private readonly BankSimDbContext _context;

        public AccountRepository(BankSimDbContext context)
        {
            _context = context;
        }

        public async Task<List<Account>> GetAllByCustomerIdAsync(int customerId)
        {
            return await _context.Accounts
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task<Account?> GetByIbanAsync(string iban)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.IBAN == iban);
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public void Delete(Account account)
        {
            _context.Accounts.Remove(account);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
