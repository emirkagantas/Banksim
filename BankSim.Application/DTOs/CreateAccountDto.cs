using BankSim.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.DTOs
{
    public class CreateAccountDto
    {
        public int CustomerId { get; set; }
        public Currency Currency { get; set; } = Currency.TL;

    }
}

