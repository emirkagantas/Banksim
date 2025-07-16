using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.Services
{
    public interface IExchangeRateService
    {
        Task<decimal> GetExchangeRateAsync(string from, string to);
    }
}
