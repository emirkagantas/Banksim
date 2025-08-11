using BankSim.Application.Utils;

namespace BankSim.Tests
{
    public class IbanGeneratorTests
    {
        [Fact]
        public void Generate_ShouldStartWithTR_AndBe20Chars()
        {
            var iban = IbanGenerator.Generate();
            Assert.StartsWith("TR", iban);
            Assert.Equal(26, iban.Length);
        }

        [Fact]
        public void Generate_ShouldBeUnique()
        {
            var iban1 = IbanGenerator.Generate();
            var iban2 = IbanGenerator.Generate();
            Assert.NotEqual(iban1, iban2);
        }
    }
}


