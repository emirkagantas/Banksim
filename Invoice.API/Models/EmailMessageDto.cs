

namespace Invoice.API.Models
{
    public class EmailMessageDto
    {
        public string To { get; set; } = null!; 
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}
