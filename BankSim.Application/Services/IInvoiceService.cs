using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSim.Application.DTOs;

namespace BankSim.Application.Services
{
    public interface IInvoiceService
    {
        Task<List<InvoiceDto>> GetInvoicesAsync(string tckn);
        Task<bool> PayInvoiceAsync(PayInvoiceRequest req,string bearerToken);
    }

}
