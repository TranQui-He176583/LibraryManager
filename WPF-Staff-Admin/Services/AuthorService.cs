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
    }
}
