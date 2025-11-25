using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
