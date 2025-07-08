using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BankSim.Application.DTOs;
using BankSim.Domain.Entities;

namespace BankSim.Application.Mapping
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();
            CreateMap<Account, AccountDto>();
            CreateMap<CreateAccountDto, Account>();
            CreateMap<Transaction, TransactionDto>().ReverseMap();

        }
    }
}

