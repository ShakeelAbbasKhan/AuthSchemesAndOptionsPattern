using Microsoft.AspNetCore.Identity;

namespace AuthSchemesAndOptionsPattern.Data
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? LastLoginDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsRevoked { get; set; }
    }
}
