using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankSim.Application.DTOs
{
    public class TransactionFilterDto
    {
   
        public int accountId { get; set; }

       
        public DateTime? startDate { get; set; }

      
        public DateTime? endDate { get; set; }
    }
}
