using BusinessLayer.DTOs;
namespace BusinessLayer.Interfaces
{
    public interface IRecruitmentService
    {
        ///resume upload
        Task<int> SaveCandidateAsync(CandidateDto dto);

        Task<IEnumerable<object>> GetCandidatesAsync(int userId, int companyId, int regionId);

        Task<bool> MoveStageAsync(int candidateId, int stageId);

        Task<bool> DeleteCandidateAsync(int candidateId);
        Task<CandidateDto?> GetCandidateByIdAsync(int candidateId);
        Task<bool> UpdateCandidateAsync(CandidateDto dto);

        Task<IEnumerable<object>> GetReferenceUsersAsync();
       


        /////////screening
        Task<IEnumerable<object>> GetRecruitersAsync();

        Task<IEnumerable<object>> GetScreeningCandidatesTopTableAsync(
   int companyId,
   int regionId,
   string department,
   string designation
);
        Task<bool> SaveCandidateScreeningAsync(CandidateScreeningDto dto);

        Task<IEnumerable<CandidateScreeningDto>> GetScreeningRecordsAsync(int userId,int companyId, int regionId);

        Task<bool> UpdateCandidateScreeningAsync(CandidateScreeningDto dto);


        //////////Interview

        Task<IEnumerable<object>> GetScreeningCandidatesTopTableInterviewAsync(
int companyId,
int regionId,
string department,
string designation
);
        Task<bool> SaveCandidateInterviewAsync(CandidateInterviewDto dto);

        Task<IEnumerable<CandidateInterviewDto>> GetInterviewRecordsAsync(int userId,
            int companyId,
            int regionId
        );
        Task<bool> UpdateCandidateInterviewAsync(CandidateInterviewDto dto);

        // Appointment Screen
        Task<IEnumerable<CandidateAppointmentDto>> GetAppointmentsForInterviewerAsync(int interviewerId);
        Task<object?> GetAppointmentCandidateDetailsAsync(int candidateId);

        /////////  Offer
        Task<IEnumerable<object>> GetOfferCandidatesTopTableAsync(
 int companyId,
 int regionId,
 string department,
 string designation
);
        Task<bool> SaveCandidateOfferAsync(CandidateOfferDto dto);

        Task<IEnumerable<CandidateOfferDto>> GetOfferRecordsAsync(
            int userId,
            int companyId,
            int regionId
        );
       Task<IEnumerable<object>> GetHRUsersAsync(int companyId, int regionId);

        Task<bool> SendOfferLetterAsync(int offerId);
        Task<(byte[] fileBytes, string fileName)> DownloadOfferLetterAsync(int offerId);


        //////////onboarding

        Task<IEnumerable<object>> GetOnboardingCandidatesTopTableAsync(
 int companyId,
 int regionId,
 string department,
 string designation
);

        Task<int> SaveCandidateOnboardingAsync(CandidateOnboardingDTO dto);

        Task<IEnumerable<object>> GetOnboardedCandidatesAsync(int companyId, int regionId);

    }
}
