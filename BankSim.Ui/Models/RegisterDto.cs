namespace BankSim.Ui.Models
{
    public class RegisterDto
    {
        public string FullName { get; set; } = "";
        public string IdentityNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = ""; 
    }

}
