using Microsoft.AspNetCore.Mvc;
using BookStore.Models;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryModel _categoryModel;

        public CategoryController(IConfiguration configuration)
        {
            _categoryModel = new CategoryModel(configuration);
        }

        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _categoryModel.GetAllCategories();
            return Ok(categories);
        }

        [HttpGet("id={id}")]
        public IActionResult GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid category ID.");
            }

            var category = _categoryModel.GetCategoryById(id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(category);
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] string name, [FromBody] int? parent_id)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Category name is required.");
            }

            if (_categoryModel.AddCategory(name, parent_id))
            {
                return Ok("Category added successfully.");
            }
            else
            {
                return BadRequest("Failed to add category.");
            }
        }

        [HttpPut("id={id}")]
        public IActionResult UpdateCategory(int id, [FromBody] string name, [FromBody] int? parent_id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid category ID.");
            }

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Category name is required.");
            }

            if (_categoryModel.UpdateCategory(id, name, parent_id))
            {
                return Ok("Category updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update category.");
            }
        }

        [HttpDelete("id={id}")]
        public IActionResult DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid category ID.");
            }

            if (_categoryModel.DeleteCategory(id))
            {
                return Ok("Category deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete category.");
            }
        }
    }
}
