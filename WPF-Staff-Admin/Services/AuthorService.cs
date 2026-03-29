using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;
namespace WPF_Staff_Admin.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IApiService _apiService;

        public AuthorService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ApiResponse<List<AuthorDTO>>> GetAllAuthorsAsync()
        {
            return await _apiService.GetAsync<List<AuthorDTO>>("Author");
        }

        public async Task<ApiResponse<AuthorDTO>> CreateAuthorAsync(AuthorDTO request)
        {
            return await _apiService.PostAsync<AuthorDTO>("Author", request);
        }

        public async Task<ApiResponse<AuthorDTO>> UpdateAuthorAsync(int id, AuthorDTO request)
        {
            return await _apiService.PutAsync<AuthorDTO>($"Author/{id}", request);
        }

        public async Task<ApiResponse<bool>> DeleteAuthorAsync(int id)
        {
            return await _apiService.DeleteAsync<bool>($"Author/{id}");
        }
    }
}
