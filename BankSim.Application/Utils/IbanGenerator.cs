using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.Utils
{
    public static class IbanGenerator
    {
        public static string Generate()
        {
            var random = new Random();
            var builder = new StringBuilder("TR");
            builder.Append(random.Next(10, 99)); 

            for (int i = 0; i < 16; i++)
            {
                builder.Append(random.Next(0, 10));
            }

            return builder.ToString();
        }
    }
}

