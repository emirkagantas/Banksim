namespace Invoice.API.Models
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Iban { get; set; } = null!;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = null!;

    }
}
