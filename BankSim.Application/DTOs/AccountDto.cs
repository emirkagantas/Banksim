using BankSim.Domain.Enums.BankSim.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string IBAN { get; set; } = null!;
        public decimal Balance { get; set; }
        public int CustomerId { get; set; }
        public Currency Currency { get; set; }
    }
}

