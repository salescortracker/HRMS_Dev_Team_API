using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IRecruitmentService
    {
        Task<int> AddCandidateAsync(CandidateCreateDto dto);
        Task<List<CandidateListDto>> GetCandidatesAsync(int userId, int roleId);
        Task AdvanceStageAsync(int candidateId, int userId);
        Task DeleteCandidateAsync(int candidateId, int userId);
        Task<IEnumerable<RecruiterDto>> GetRecruitersAsync();

        Task<List<CandidateListDto>> GetScreeningCandidatesAsync(int userId, int roleId);
        Task SaveCandidateScreeningAsync(CandidateScreeningCreateDto dto);

        Task<List<CandidateScreeningListDto>> GetScreeningRecordsAsync(
    int companyId,
    int regionId
);


    }
}
