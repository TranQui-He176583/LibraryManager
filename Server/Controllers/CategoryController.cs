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
        public ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<List<CategoryDTO>>>> GetAllCategory()
        {
            return await _categoryService.GetAllCategory();
        }
    }
}
