using System.ComponentModel.DataAnnotations;

namespace AuthSchemesAndOptionsPattern.Helper
{
    public class AppSettings
    {
        public const string ConnectionStrings = "ConnectionStrings";
        public const string JWTKey = "JWTKey";
       
        [Required]
        public string DefaultConnection { get; set; }
        [Range(5, 30)]
        public string ExampleValue { get; set; }

        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string TokenExpiryTimeInHour { get; set; }
        public int RefreshTokenValidityInDays { get; set; }
        public string Secret { get; set; }

        public string AuthenticationMethod { get; set; }

    }
}
