using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int ToAccountId { get; set; }
        public int FromAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public Account FromAccount { get; set; } = null!;
        public Account ToAccount { get; set; } = null!;
    }
}
