using BankSim.Application.Utils;

namespace BankSim.Tests
{
    public class IbanTests
    {
        [Fact]
        public void Generate_ShouldReturnStringStartingWithTR_And20Chars()
        {

            var iban = IbanGenerator.Generate();


            Assert.StartsWith("TR", iban);
            Assert.Equal(20, iban.Length);
        }
    }
}


