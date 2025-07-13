namespace BankSim.Ui.Models
{
    public class TransactionDto
    {
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
