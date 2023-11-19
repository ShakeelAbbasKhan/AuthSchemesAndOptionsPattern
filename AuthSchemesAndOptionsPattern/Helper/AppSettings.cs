using System.ComponentModel.DataAnnotations;

namespace AuthSchemesAndOptionsPattern.Helper
{
    public class AppSettings
    {
        public const string ConnectionStrings = "ConnectionStrings";
        [Required]
        public string DefaultConnection { get;set;}
        [Range(5,30)]
        public string ExampleValue { get; set; }

    }
}
