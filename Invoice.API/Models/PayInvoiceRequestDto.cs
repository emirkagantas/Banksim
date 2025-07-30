namespace Invoice.API.Models
{
    public class PayInvoiceRequestEntity
    {
        public int InvoiceId { get; set; }
        public string CustomerTckn { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
    }
}
