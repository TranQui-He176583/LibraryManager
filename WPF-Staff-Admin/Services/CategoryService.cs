using System;
using System.Collections.Generic;
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

        public async Task<ApiResponse<CategoryDTO>> CreateCategoryAsync(CategoryDTO category)
        {
            return await _apiService.PostAsync<CategoryDTO>("Category", category);
        }

        public async Task<ApiResponse<CategoryDTO>> UpdateCategoryAsync(int id, CategoryDTO category)
        {
            return await _apiService.PutAsync<CategoryDTO>($"Category/{id}", category);
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
        {
            return await _apiService.DeleteAsync<bool>($"Category/{id}");
        }
    }
}
