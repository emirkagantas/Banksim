

    namespace BankSim.Ui.Models
    {
        public class InvoiceDto
        {
            public int Id { get; set; }
            public string CustomerTckn { get; set; } = null!;
            public string Type { get; set; } = null!;    
            public decimal Amount { get; set; }
            public DateTime DueDate { get; set; }
            public bool IsPaid { get; set; }
        }
    }


