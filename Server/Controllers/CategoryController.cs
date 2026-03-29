using Server.Models;
using Server.DTOs;
using Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<List<CategoryDTO>>>> GetAllCategory()
        {
            return await _categoryService.GetAllCategory();
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<CategoryDTO>>> Create([FromBody] CategoryDTO request)
        {
            return await _categoryService.CreateCategory(request);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDTO<CategoryDTO>>> Update(int id, [FromBody] CategoryDTO request)
        {
            return await _categoryService.UpdateCategory(id, request);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDTO<bool>>> Delete(int id)
        {
            return await _categoryService.DeleteCategory(id);
        }
    }
}
