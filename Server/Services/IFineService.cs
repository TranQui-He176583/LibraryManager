using Server.DTOs;

namespace Server.Services
{
    public interface IFineService
    {
        Task<List<FineDTO>> GetAllFinesAsync();
        Task<List<FineDTO>> GetFinesByMemberAsync(int memberId);
        Task<FineDTO?> GetFineByIdAsync(int fineId);
        Task<bool> MarkAsPaidAsync(int fineId);
        Task<FineDTO> CreateFineAsync(FineDTO fineDto);
    }
}
