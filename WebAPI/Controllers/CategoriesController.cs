// CategoriesController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos.Category;
using Data.Entities.Category;
using Service.IServices;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(string id)
        {
            var category = await _categoryService.GetCategory(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CreateCategoryDTO createCategoryDTO)
        {
            var createdCategory = await _categoryService.CreateCategory(createCategoryDTO);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(string id, CreateCategoryDTO createCategoryDTO)
        {
            await _categoryService.UpdateCategory(id, createCategoryDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _categoryService.DeleteCategory(id);
            return NoContent();
        }
    }
}