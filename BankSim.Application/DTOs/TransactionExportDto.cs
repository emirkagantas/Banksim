using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.DTOs
{
    public class TransactionExportDto
    {
        public string FromIban { get; set; } = "";
        public string FromFullName { get; set; } = "";
        public string ToIban { get; set; } = "";
        public string ToFullName { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ConvertedAmount { get; set; }
    }

}
