namespace BankSim.Ui.Models
{
    public class TransactionDto
    {
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
        
        public string ToAccountName { get; set; } = string.Empty;
        public string FromAccountName { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }
}
