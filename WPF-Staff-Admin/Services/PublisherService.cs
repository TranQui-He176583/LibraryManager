using System.Collections.Generic;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IApiService _apiService;

        public PublisherService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ApiResponse<List<PublisherDTO>>> GetAllPublishersAsync()
        {
            return await _apiService.GetAsync<List<PublisherDTO>>("Publisher");
        }

        public async Task<ApiResponse<PublisherDTO>> CreatePublisherAsync(PublisherDTO publisher)
        {
            return await _apiService.PostAsync<PublisherDTO>("Publisher", publisher);
        }

        public async Task<ApiResponse<PublisherDTO>> UpdatePublisherAsync(int id, PublisherDTO publisher)
        {
            return await _apiService.PutAsync<PublisherDTO>($"Publisher/{id}", publisher);
        }

        public async Task<ApiResponse<bool>> DeletePublisherAsync(int id)
        {
            return await _apiService.DeleteAsync<bool>($"Publisher/{id}");
        }
    }
}
