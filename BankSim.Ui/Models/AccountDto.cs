namespace BankSim.Ui.Models 
{
  
    public class AccountDto
    {
        public int Id { get; set; }             
        public string? IBAN { get; set; }       
        public decimal Balance { get; set; }     
        public string? Currency { get; set; }     
    }

}
