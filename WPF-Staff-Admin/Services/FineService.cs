using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
    public class FineService : IFineService
    {
        private readonly IApiService _apiService;

        public FineService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<FineDTO>> GetAllFinesAsync()
        {
            var response = await _apiService.GetAsync<List<FineDTO>>("fines");
            return response.Data ?? new List<FineDTO>();
        }

        public async Task<List<FineDTO>> GetMemberFinesAsync(int memberId)
        {
            var response = await _apiService.GetAsync<List<FineDTO>>($"fines/member/{memberId}");
            return response.Data ?? new List<FineDTO>();
        }

        public async Task<bool> PayFineAsync(int fineId)
        {
            var response = await _apiService.PostAsync($"fines/{fineId}/pay", null);
            return response.Success;
        }

        public async Task<ApiResponse> ReportIssueAsync(ReportIssueRequest request)
        {
            var response = await _apiService.PostAsync("fines/report-issue", request);
            return response ?? new ApiResponse { Success = false, Message = "Lỗi kết nối Server" };
        }
    }
}
