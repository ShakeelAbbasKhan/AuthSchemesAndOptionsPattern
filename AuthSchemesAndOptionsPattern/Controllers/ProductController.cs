using AuthSchemesAndOptionsPattern.Data;
using AuthSchemesAndOptionsPattern.Dtos;
using AuthSchemesAndOptionsPattern.Helper;
using AuthSchemesAndOptionsPattern.Model;
using AuthSchemesAndOptionsPattern.Repository;
using AutoMapper;
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
        private readonly AppSettings _optionsEager;
        private readonly AppSettings _optionsSnapshotEager;
        private readonly AppSettings _optionsMonitorEager;

        private readonly IOptions<AppSettings> _options;
        private readonly IOptionsSnapshot<AppSettings> _optionsSnapshot;
        private readonly IOptionsMonitor<AppSettings> _optionsMonitor;

        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(ApplicationDbContext context,IOptions<AppSettings> options,IOptionsSnapshot<AppSettings> optionsSnapshot,IOptionsMonitor<AppSettings> optionsMonitor, IProductRepository productRepository,IMapper mapper)
        {
            _context = context;
            _options = options;
            _optionsEager = options.Value;
            _optionsSnapshotEager = optionsSnapshot.Value;
            _optionsMonitorEager = optionsMonitor.CurrentValue;

            _optionsSnapshot = optionsSnapshot;
            _optionsMonitor = optionsMonitor;
            _productRepository = productRepository;
            _mapper = mapper;

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


        [HttpGet("GetDataEager")]
        public async Task<IActionResult> GetDataEager()
        {
            // var dbValue = _options.Value.DefaultConnection;
            var optionsValueEager = _optionsEager.ExampleValue;
            var optionsSnapShotValueEager = _optionsSnapshotEager.ExampleValue;
            var optionsMonitorValueEager = _optionsMonitorEager.ExampleValue;

            var responseMessage = $"Option Value: {optionsValueEager}, SnapShot Value: {optionsSnapShotValueEager}, Monitor Value: {optionsMonitorValueEager}";

            return Ok(responseMessage);
        }


        [HttpGet("GetDataExplicit")]
        public async Task<IActionResult> GetDataExplicit()
        {
            // var dbValue = _options.Value.DefaultConnection;
            var optionsValueExplicit = _options.Value.ExampleValue;
            var optionsSnapShotValueExplicit = _optionsSnapshot.Value.ExampleValue;
            var optionsMonitorValueExplicit = _optionsMonitor.CurrentValue.ExampleValue;

            var responseMessage = $"Option Value: {optionsValueExplicit}, SnapShot Value: {optionsSnapShotValueExplicit}, Monitor Value: {optionsMonitorValueExplicit}";

            return Ok(responseMessage);
        }

        [HttpGet("GetDataLazyLoading")]
        public async Task<IActionResult> GetDataLazyLoading()
        {
            var optionsSnapShotValue = _optionsSnapshot.Value.ExampleValue;
            var optionsMonitorValue = _optionsMonitor.CurrentValue.ExampleValue;

            var responseMessage = $"SnapShot Value: {optionsSnapShotValue}, Monitor Value: {optionsMonitorValue}";
            
            return Ok(responseMessage);
        }


        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var std = await _productRepository.GetProductsAsync();

            return Ok(_mapper.Map<List<ProductDto>>(std));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var cat = await _productRepository.GetProductAsync(id);
            if (cat == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductDto>(cat));
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cat = await _productRepository.AddProductAsync(_mapper.Map<Product>(createProductDto));


            return CreatedAtAction("GetProduct", new { id = cat.Id }, _mapper.Map<CreateProductDto>(cat));
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto updateProductDto)
        {
            var updateCatExist = await _productRepository.GetProductAsync(id);
            if (updateCatExist != null)
            {
                // update method
                var orignalCat = _mapper.Map<Product>(updateProductDto);    // orignalStd b/c give to db so it is destination
                var updatedCat = await _productRepository.UpdateProductAsync(id, orignalCat);

                if (updatedCat != null)
                {
                    return Ok(_mapper.Map<ProductDto>(updatedCat));     // here studentDto b/c return to user
                }
            }

            return NotFound();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deletedCat = await _productRepository.DeleteProductAsync(id);
            if (deletedCat == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductDto>(deletedCat));
        }
    }
}
