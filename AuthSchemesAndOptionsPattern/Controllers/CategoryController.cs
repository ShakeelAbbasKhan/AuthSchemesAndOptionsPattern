using AutoMapper;
using AuthSchemesAndOptionsPattern.Data;
using AuthSchemesAndOptionsPattern.Dtos;
using AuthSchemesAndOptionsPattern.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthSchemesAndOptionsPattern.Repository;

namespace AuthSchemesAndOptionsPattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ApplicationDbContext context, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _context = context;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var std = await _categoryRepository.GetCategoriesAsync();

            return Ok(_mapper.Map<List<CategoryDto>>(std));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var cat = await _categoryRepository.GetCategoryAsync(id);
            if (cat == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CategoryDto>(cat));
        }



        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cat = await _categoryRepository.AddCategoryAsync(_mapper.Map<Category>(categoryDto));


            return CreatedAtAction("GetCategory", new { id = cat.Id }, _mapper.Map<CreateCategoryDto>(cat));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            var updateCatExist = await _categoryRepository.GetCategoryAsync(id);
            if (updateCatExist != null)
            {
                // update method
                var orignalCat = _mapper.Map<Category>(updateCategoryDto);    // orignalStd b/c give to db so it is destination
                var updatedCat = await _categoryRepository.UpdateCategoryAsync(id, orignalCat);

                if (updatedCat != null)
                {
                    return Ok(_mapper.Map<CategoryDto>(updatedCat));     // here studentDto b/c return to user
                }
            }

            return NotFound();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deletedCat = await _categoryRepository.DeleteCategoryAsync(id);
            if (deletedCat == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CategoryDto>(deletedCat));
        }
    }


}
