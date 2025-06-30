using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankSim.Domain.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string IBAN { get; set; } = null!;
        public decimal Balance { get; set; }
        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;
        public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();




    }
}
