using BankSim.Domain.Entities;
using BankSim.Domain.Interfaces;
using BankSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSim.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly BankSimDbContext _context;

        public CustomerRepository(BankSimDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
         
        }

        public async Task UpdateAsync(Customer customer)
        {
             _context.Customers.Update(customer);
       
        }

        public async Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            
        }
    }
}
