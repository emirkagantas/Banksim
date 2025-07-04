﻿using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string IdentityNumber { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public ICollection<Account> Accounts { get; set; } = new List<Account>();


     


    }
}
