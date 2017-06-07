using Microsoft.IdentityModel.Tokens;

namespace Tunr.Utilities
{
    public class TokenAuthOptions

    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
    }
}