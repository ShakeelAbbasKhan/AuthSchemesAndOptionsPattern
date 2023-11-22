using AuthSchemesAndOptionsPattern.Data;
using AuthSchemesAndOptionsPattern.Helper;
using AuthSchemesAndOptionsPattern.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Data;

namespace AuthSchemesAndOptionsPattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTService _jWTService;
        private readonly IConfiguration _configuration;
        private readonly IOptionsSnapshot<AppSettings> _optionsSnapshot;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, JWTService jWTService, IConfiguration configuration, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jWTService = jWTService;
            _optionsSnapshot = optionsSnapshot;
        }

        //[HttpPost("Login")]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    TokenViewModel _TokenViewModel = new();
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);
        //        if (user != null && user.LastLoginDate.HasValue)
        //        {
        //            DateTime lastLoginDate = user.LastLoginDate.Value; // Convert to DateTime
        //            if (lastLoginDate.AddDays(5) < DateTime.UtcNow)
        //            {
        //                return Ok("Password Expires Reset the Password");
        //            }
        //        }

        //        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
        //        if (result.Succeeded)
        //        {
        //            _TokenViewModel.AccessToken = await _jWTService.GenerateTokenString(model);
        //            _TokenViewModel.StatusCode = 1;
        //            _TokenViewModel.StatusMessage = "Success";

        //            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWTKey:RefreshTokenValidityInDays"]);
        //            user.RefreshToken = _TokenViewModel.AccessToken;
        //            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);

        //            user.LastLoginDate = DateTime.Now;
        //            await _userManager.UpdateAsync(user);

        //            return Ok(new { _TokenViewModel });

        //        }

        //        if (result.IsLockedOut)
        //        {
        //            var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
        //            return BadRequest($"Your account is locked out until {lockoutEndDate}.");
        //        }

        //        return BadRequest("Invalid login attempt");
        //    }

        //    return BadRequest(ModelState);
        //}

        [HttpPost("LoginJWT")]
        public async Task<IActionResult> LoginJWT(LoginViewModel model)
        {
            TokenViewModel _TokenViewModel = new TokenViewModel();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && user.LastLoginDate.HasValue)
                {
                    DateTime lastLoginDate = user.LastLoginDate.Value;

                    if (lastLoginDate.AddDays(5) < DateTime.UtcNow)
                    {
                        return Ok("Password Expires Reset the Password");
                    }
                }

                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

                    if (result.Succeeded)
                    {


                        {
                            // Sign in using JWT Authentication
                            _TokenViewModel.AccessToken = await _jWTService.GenerateTokenString(model);
                            _TokenViewModel.StatusCode = 1;
                            _TokenViewModel.StatusMessage = "Success";

                            return Ok(new { _TokenViewModel });
                        }
                    
                    }

                    if (result.IsLockedOut)
                    {
                        var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
                        return BadRequest($"Your account is locked out until {lockoutEndDate}.");
                    }

                    return BadRequest("Invalid login attempt");

                }

                else
                {
                    return BadRequest("Invalid authentication method specified");
                }


            return BadRequest(ModelState);
        }


        //Generate Cookies When Logged In
        [HttpPost("LoginCookies")]
        public async Task<IActionResult> LoginCookies([FromBody] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),

                    };

                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Any())
                        {
                            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                        }

            var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        AllowRefresh = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return Ok(new { message = "Login successful." });
                }

                return BadRequest(new { message = "Invalid login attempt." });
            }

            return BadRequest(new { message = "Invalid model state" });
        }



        // [Authorize(Roles = "Admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = registerModel.Email,
                    Email = registerModel.Email,
                    LastLoginDate = DateTime.Now,
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, registerModel.RoleName);

                    return Ok("Registration successful");
                }

                return BadRequest(result.Errors);
            }

            return BadRequest(ModelState);
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
           

           // await _signInManager.SignOutAsync();
            return Ok("Logged out successfully");
        }

        //   [Authorize(Policy = "SuperUserRights")]
        [HttpGet("UserList")]
        public IActionResult UserList()
        {
            var users = _userManager.Users.Select(u => new UserListVM
            {
                Id = u.Id,
                Email = u.Email,
                Roles = _userManager.GetRolesAsync(u).Result.ToList()
            }).ToList();

            return Ok(users);
        }

    }
}
