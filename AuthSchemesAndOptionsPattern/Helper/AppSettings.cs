using System.ComponentModel.DataAnnotations;

namespace AuthSchemesAndOptionsPattern.Helper
{
    public class AppSettings
    {
        public const string ConnectionStrings = "ConnectionStrings";
        public  string JWTKey { get;} = "JWTKey";
       
        [Required]
        public string DefaultConnection { get; set; }
        [Range(5, 30)]
        public string ExampleValue { get; set; }

        public string ValidAudience { get; }
        public string ValidIssuer { get; set; }
        public string TokenExpiryTimeInHour { get; set; }
        public int RefreshTokenValidityInDays { get; set; }
        public string Secret { get; set; }

        public string AuthenticationMethod { get; set; }

    }
}
