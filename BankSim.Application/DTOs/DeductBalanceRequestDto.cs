using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.DTOs
{
    public class DeductBalanceRequest
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
    }

}
