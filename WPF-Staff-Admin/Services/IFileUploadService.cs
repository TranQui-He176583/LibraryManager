using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;
namespace WPF_Staff_Admin.Services
{
    public interface IFileUploadService
    {
        Task<ApiResponse<string>> UploadBookImageAsync(string filePath);
        Task<ApiResponse> DeleteBookImageAsync(int? bookId);
    }

}
