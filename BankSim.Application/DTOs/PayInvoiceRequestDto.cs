using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.DTOs
{
    public class PayInvoiceRequest
    {
        public int InvoiceId { get; set; }
        public string CustomerTckn { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
    }

}
