using DocumentFormat.OpenXml.Office.CoverPageProps;

namespace Invoice.API.Models
{
    public class InvoiceEntity
    {
        public int Id { get; set; }

        public string CustomerEmail { get; set; } = null!;

        public string CustomerTckn { get; set; } = null!;
        
        public string Type { get; set; } = null!;

        public decimal Amount { get; set; }
        
        public DateTime DueDate { get; set; }

        public bool IsPaid { get; set; }


    }
}
