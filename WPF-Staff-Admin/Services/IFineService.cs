using System.Collections.Generic;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
    public interface IFineService
    {
        Task<List<FineDTO>> GetAllFinesAsync();
        Task<List<FineDTO>> GetMemberFinesAsync(int memberId);
        Task<bool> PayFineAsync(int fineId);
        Task<ApiResponse> ReportIssueAsync(ReportIssueRequest request);
    }
}
