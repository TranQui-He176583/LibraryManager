using Microsoft.AspNetCore.Mvc;
using Server.DTOs;

namespace Server.Services
{
    public interface ICategoryService
    {

        Task<ActionResult<ApiResponseDTO<List<CategoryDTO>>>> GetAllCategory();
    }
        
}
