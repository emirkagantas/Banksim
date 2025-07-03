using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BankSim.Application.DTOs;

namespace BankSim.Application.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllAsync();
        Task<CustomerDto?> GetByIdAsync(int id);
        Task AddAsync(CreateCustomerDto dto);
        Task UpdateAsync(int id, UpdateCustomerDto dto);
        Task DeleteAsync(int id);
    }
}

