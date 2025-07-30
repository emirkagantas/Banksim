namespace BankSim.Ui.Models
{
    public class PayInvoiceRequestDto
    {
        public int InvoiceId { get; set; }
        public string CustomerTckn { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
    }
}
