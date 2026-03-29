using Microsoft.AspNetCore.Mvc;
using Server.DTOs;

namespace Server.Services
{
    public interface ICategoryService
    {
        Task<ActionResult<ApiResponseDTO<List<CategoryDTO>>>> GetAllCategory();
        Task<ActionResult<ApiResponseDTO<CategoryDTO>>> CreateCategory(CategoryDTO request);
        Task<ActionResult<ApiResponseDTO<CategoryDTO>>> UpdateCategory(int id, CategoryDTO request);
        Task<ActionResult<ApiResponseDTO<bool>>> DeleteCategory(int id);
    }
}
