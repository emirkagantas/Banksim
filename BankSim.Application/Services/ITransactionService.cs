using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSim.Application.DTOs;

namespace BankSim.Application.Services
{
    public interface ITransactionService
    {
        Task TransferAsync(TransactionDto dto);
        Task<List<TransactionDto>> GetByAccountIdAsync(int accountId);
        Task<List<TransactionDto>> GetByFilterAsync(TransactionFilterDto filter);
        Task<List<TransactionExportDto>> GetExportListAsync(TransactionFilterDto filter);

    }
}
