using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthSchemesAndOptionsPattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        //  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Authorize(Policy = "CookiePolicy")]
        [HttpGet("GetByCookie")]
        public IActionResult GetByCookie()
        {
            return Ok("you hit by cookie");
        }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "JwtPolicy")]
        [HttpGet("GetByJWT")]
        
        public IActionResult GetByJWT()
        {
            return Ok("you hit by jwt");
        }
    }
}
