using BankSim.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BankSim.Application.Services
{
    public interface IAuthService
{
    Task RegisterAsync(RegisterDto dto);
    Task<string?> LoginAsync(LoginDto dto);
}
}

