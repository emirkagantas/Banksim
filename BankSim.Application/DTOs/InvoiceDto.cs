using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string CustomerTckn { get; set; } =null!;
        public string Type { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
    }

}
