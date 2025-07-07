using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankSim.Domain.Entities;

namespace BankSim.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
    }
}       
