using AuthSchemesAndOptionsPattern.Data;
using AuthSchemesAndOptionsPattern.Helper;
using AuthSchemesAndOptionsPattern.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthSchemesAndOptionsPattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<AppSettings> _options;
        private readonly IOptionsSnapshot<AppSettings> _optionsSnapshot;
        private readonly IOptionsMonitor<AppSettings> _optionsMonitor;

        public ProductController(ApplicationDbContext context,IOptions<AppSettings> options,IOptionsSnapshot<AppSettings> optionsSnapshot,IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _context = context;
            _options = options;
            _optionsSnapshot = optionsSnapshot;
            _optionsMonitor = optionsMonitor;


        }
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

        [HttpGet("GetDataAppSettings")]
        public async Task<IActionResult> GetDataAppSettings()
        {
            // var dbValue = _options.Value.DefaultConnection;
            var optionsValue = _options.Value.ExampleValue;
            var optionsSnapShotValue = _optionsSnapshot.Value.ExampleValue;
            var optionsMonitorValue = _optionsMonitor.CurrentValue.ExampleValue;

            var responseMessage = $"Option Value: {optionsValue}, SnapShot Value: {optionsSnapShotValue}, Monitor Value: {optionsMonitorValue}";

            return Ok(responseMessage);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
          
             var product = await _context.Products.ToListAsync();
            return Ok(product);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
              var product = await _context.Products.FirstOrDefaultAsync(u => u.Id == id);
            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();

            return Ok(product);
        }
    }
}
