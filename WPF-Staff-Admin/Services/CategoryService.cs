using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;
namespace WPF_Staff_Admin.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApiService _apiService;

        public CategoryService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ApiResponse<List<CategoryDTO>>> GetAllCategoriesAsync()
        {
            return await _apiService.GetAsync<List<CategoryDTO>>("Category");
        }
    }
}
