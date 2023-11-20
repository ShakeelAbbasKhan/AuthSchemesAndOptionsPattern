using AuthSchemesAndOptionsPattern.Data;
using AuthSchemesAndOptionsPattern.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthSchemesAndOptionsPattern.Helper
{
    public class TestService
    {
        private readonly IConfiguration _configuration;
        private readonly IOptionsSnapshot<AppSettings> _optionsSnapshot;
        private readonly IOptionsMonitor<AppSettings> _optionsMonitor;


        public TestService(IConfiguration configuration, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _configuration = configuration;
            // _optionsSnapshot = optionsSnapshot;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<string> GenerateTokenString(LoginViewModel model)
        {


            // var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));

            //  var checkValue = _optionsSnapshot.Value;
           // var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_optionsSnapshot.Value.Secret));
            var checkValue = _optionsMonitor.CurrentValue;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_optionsMonitor.CurrentValue.Secret));


            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(60),
                issuer: _configuration["JWTKey:ValidIssuer"],
                audience: _configuration["JWTKey:ValidAudience"],
                signingCredentials: signingCred);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }
    }
}
