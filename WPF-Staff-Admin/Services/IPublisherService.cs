using System.Collections.Generic;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
    public interface IPublisherService
    {
        Task<ApiResponse<List<PublisherDTO>>> GetAllPublishersAsync();
        Task<ApiResponse<PublisherDTO>> CreatePublisherAsync(PublisherDTO publisher);
        Task<ApiResponse<PublisherDTO>> UpdatePublisherAsync(int id, PublisherDTO publisher);
        Task<ApiResponse<bool>> DeletePublisherAsync(int id);
    }
}
