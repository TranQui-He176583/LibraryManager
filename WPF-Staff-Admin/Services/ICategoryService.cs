using System.Collections.Generic;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryDTO>>> GetAllCategoriesAsync();
        Task<ApiResponse<CategoryDTO>> CreateCategoryAsync(CategoryDTO category);
        Task<ApiResponse<CategoryDTO>> UpdateCategoryAsync(int id, CategoryDTO category);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
    }
}
