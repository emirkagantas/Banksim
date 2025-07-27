namespace BankSim.Ui.Models
{
    public class DashboardDto
    {
        public CustomerDto Customer { get; set; } = null!;
        public int AccountCount { get; set; }
        public decimal TotalBalance { get; set; }
        public List<TransactionDto> LastTransactions { get; set; } = new();
    }
}
