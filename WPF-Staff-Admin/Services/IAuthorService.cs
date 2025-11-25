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
    }
}
