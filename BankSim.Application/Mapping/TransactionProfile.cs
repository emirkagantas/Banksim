using AutoMapper;
using BankSim.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSim.Domain.Entities;

namespace BankSim.Application.Mapping
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionDto>().ReverseMap();
            CreateMap<Transaction, TransactionExportDto>();

        }

    }
    
    
}
