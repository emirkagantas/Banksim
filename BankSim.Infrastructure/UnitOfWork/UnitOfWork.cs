using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSim.Domain.Interfaces;
using BankSim.Infrastructure.Persistence;

namespace BankSim.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankSimDbContext _context;

        public UnitOfWork(BankSimDbContext context)
        {
            _context = context;
        }
       

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
