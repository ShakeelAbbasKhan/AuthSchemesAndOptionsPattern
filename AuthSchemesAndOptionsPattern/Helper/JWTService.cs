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
    public class JWTService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptionsSnapshot<AppSettings> _optionsSnapshot;
        private readonly AppSettings _options;


        public JWTService(IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptionsSnapshot<AppSettings> optionsSnapshot, IOptions<AppSettings> options)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _optionsSnapshot = optionsSnapshot;
            _options = options.Value;
        }

        public async Task<string> GenerateTokenString(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var authClaims = new List<Claim>();

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                authClaims.AddRange(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                });

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
            }

            // var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
            var checkValue = _optionsSnapshot.Value;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_optionsSnapshot.Value.Secret));


            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(60),
                //issuer: _configuration["JWTKey:ValidIssuer"],
                //audience: _configuration["JWTKey:ValidAudience"],
                 issuer: _optionsSnapshot.Value.ValidIssuer,
                audience: _optionsSnapshot.Value.ValidAudience,
                signingCredentials: signingCred);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }
    }
}
