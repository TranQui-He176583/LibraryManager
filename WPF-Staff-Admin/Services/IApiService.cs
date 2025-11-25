using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;
namespace WPF_Staff_Admin.Services
{
    public interface IApiService
    {
        Task<ApiResponse<T>> GetAsync<T>(string endpoint);
        Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? data = null);
        Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data);
        Task<ApiResponse<T>> DeleteAsync<T>(string endpoint);
        Task<ApiResponse> PostAsync(string endpoint, object? data = null);
        Task<ApiResponse> PutAsync(string endpoint, object data);
        Task<ApiResponse> DeleteAsync(string endpoint);
    }
}
