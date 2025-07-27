using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.DTOs
{
    public class TransactionDto
    {
        public int FromAccountId { get; set; }
        public string? FromAccountName { get; set; }
        public int ToAccountId { get; set; }
        public string? ToAccountName { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
       
    }
}

