using System.IdentityModel.Tokens.Jwt;

namespace BankSim.Ui.Helpers
{
  
    public static class JwtHelper
    {
        public static string? GetClaim(string? token, string claimType)
        {
            if (string.IsNullOrEmpty(token)) return null;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }

}
