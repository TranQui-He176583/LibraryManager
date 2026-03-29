using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;
namespace WPF_Staff_Admin.Services
{
    public interface IAuthorService
    {
        Task<ApiResponse<List<AuthorDTO>>> GetAllAuthorsAsync();
        Task<ApiResponse<AuthorDTO>> CreateAuthorAsync(AuthorDTO request);
        Task<ApiResponse<AuthorDTO>> UpdateAuthorAsync(int id, AuthorDTO request);
        Task<ApiResponse<bool>> DeleteAuthorAsync(int id);
    }
}
