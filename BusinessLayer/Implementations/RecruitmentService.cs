using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using iText.Kernel.Pdf;
using iTextSharp.tool.xml;
using Newtonsoft.Json;



namespace BusinessLayer.Implementations
{
    public class RecruitmentService : IRecruitmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
       

        public RecruitmentService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
           
        }

        public async Task<int> SaveCandidateAsync(CandidateDto dto)
        {
            int year = DateTime.Now.Year;

            var lastSeq = (await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => x.CompanyId == dto.CompanyId && x.CreatedAt.Year == year))
                .OrderByDescending(x => x.CandidateId)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastSeq != null && lastSeq.SeqNo != null)
            {
                var parts = lastSeq.SeqNo.Split('_'); // REC_2026_0005
                if (parts.Length == 3)
                    nextNumber = int.Parse(parts[2]) + 1;
            }

            dto.SeqNo = $"Seq_{year}_{nextNumber.ToString("D4")}";

            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var candidate = new Candidate
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    SeqNo = dto.SeqNo,
                    StageId = dto.StageId,
                    AppliedDate = dto.AppliedDate.HasValue
                        ? DateOnly.FromDateTime(dto.AppliedDate.Value)
                        : null,

                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Mobile = dto.Mobile,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth.HasValue
                        ? DateOnly.FromDateTime(dto.DateOfBirth.Value)
                        : null,

                    MaritalStatus = dto.MaritalStatus,
                    CurrentSalary = dto.CurrentSalary,
                    ExpectedSalary = dto.ExpectedSalary,
                    ReferenceSource = dto.ReferenceSource,
                    Department = dto.Department,
                    Designation = dto.Designation,
                    Skills = dto.Skills,
                    NoticePeriod = dto.NoticePeriod,
                    AnyOffers = dto.AnyOffers,
                    Location = dto.Location,
                    Reason = dto.Reason,

                    FileName = dto.FileName,
                    FilePath = dto.FilePath,

                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = dto.UserId
                };

                await _unitOfWork.Repository<Candidate>().AddAsync(candidate);
                await _unitOfWork.CompleteAsync();

                // 🔹 EXPERIENCE
                if (!string.IsNullOrEmpty(dto.ExperiencesJson))
                {
                    var experiences = JsonConvert
                        .DeserializeObject<List<CandidateExperienceDto>>(dto.ExperiencesJson);

                    var expEntities = experiences!.Select(e => new CandidateExperience
                    {
                        CandidateId = candidate.CandidateId,
                        RegionId = dto.RegionId,
                        CompanyId = dto.CompanyId,
                        UserId = dto.UserId,
                        FromYear = e.FromYear,
                        ToYear = e.ToYear,
                        Designation = e.Designation,
                        Organization = e.Organization,
                        CreatedAt = DateTime.Now,
                        CreatedBy = dto.UserId
                    });

                    await _unitOfWork.Repository<CandidateExperience>()
                        .AddRangeAsync(expEntities);
                }

                // 🔹 QUALIFICATION
                if (!string.IsNullOrEmpty(dto.QualificationsJson))
                {
                    var qualifications = JsonConvert
                        .DeserializeObject<List<CandidateQualificationDto>>(dto.QualificationsJson);

                    var qualEntities = qualifications!.Select(q => new CandidateQualification
                    {
                        CandidateId = candidate.CandidateId,
                        RegionId = dto.RegionId,
                        CompanyId = dto.CompanyId,
                        UserId = dto.UserId,
                        FromYear = q.FromYear,
                        ToYear = q.ToYear,
                        Qualification = q.Qualification,
                        BoardUniversity = q.BoardUniversity,
                        CreatedAt = DateTime.Now,
                        CreatedBy = dto.UserId
                    });

                    await _unitOfWork.Repository<CandidateQualification>()
                        .AddRangeAsync(qualEntities);
                }

                await _unitOfWork.CompleteAsync();
                await tx.CommitAsync();

                return candidate.CandidateId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // 🔹 GET CANDIDATES
        public async Task<IEnumerable<object>> GetCandidatesAsync(int userId, int companyId, int regionId)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => x.UserId == userId && x.IsActive);

            var stages = await _unitOfWork.Repository<StageMaster>().GetAllAsync();

            return candidates.Select(c =>
            {
                var stage = stages.First(s => s.StageId == c.StageId);

                return new
                {
                    c.CandidateId,
                    c.SeqNo,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.Mobile,
                    c.Designation,
                    c.AppliedDate,
                    c.FileName,
                    StageName = stage.StageName,
                    Progress = stage.ProgressPct
                };
            });
        }

        // 🔹 MOVE STAGE
        public async Task<bool> MoveStageAsync(int candidateId, int stageId)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(candidateId);

            if (candidate == null) return false;

            candidate.StageId = stageId;
            candidate.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<Candidate>().Update(candidate);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteCandidateAsync(int candidateId)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(candidateId);

            if (candidate == null)
                return false;

            // 🔥 Hard delete
            _unitOfWork.Repository<Candidate>().Remove(candidate);

            // Optional: also delete child records explicitly if cascade is not enabled
            var exp = await _unitOfWork.Repository<CandidateExperience>()
                .FindAsync(x => x.CandidateId == candidateId);
            if (exp.Any())
                _unitOfWork.Repository<CandidateExperience>().RemoveRange(exp);

            var qual = await _unitOfWork.Repository<CandidateQualification>()
                .FindAsync(x => x.CandidateId == candidateId);
            if (qual.Any())
                _unitOfWork.Repository<CandidateQualification>().RemoveRange(qual);

            await _unitOfWork.CompleteAsync();
            return true;
        }



        public async Task<CandidateDto?> GetCandidateByIdAsync(int candidateId)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(candidateId);

            if (candidate == null) return null;

            var experiences = await _unitOfWork.Repository<CandidateExperience>()
                .FindAsync(x => x.CandidateId == candidateId);

            var qualifications = await _unitOfWork.Repository<CandidateQualification>()
                .FindAsync(x => x.CandidateId == candidateId);

            return new CandidateDto
            {
                CandidateId = candidate.CandidateId,
                AppliedDate = candidate.AppliedDate?.ToDateTime(TimeOnly.MinValue),
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                Email = candidate.Email,
                Mobile = candidate.Mobile,
                Gender = candidate.Gender,
                DateOfBirth = candidate.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
                MaritalStatus = candidate.MaritalStatus,
                CurrentSalary = candidate.CurrentSalary,
                ExpectedSalary = candidate.ExpectedSalary,
                ReferenceSource = candidate.ReferenceSource,
                Department = candidate.Department,
                Designation = candidate.Designation,
                Skills = candidate.Skills,
                NoticePeriod = candidate.NoticePeriod,
                AnyOffers = candidate.AnyOffers,
                Location = candidate.Location,
                Reason = candidate.Reason,
                Experiences = experiences.Select(e => new CandidateExperienceDto
                {
                    FromYear = e.FromYear,
                    ToYear = e.ToYear,
                    Designation = e.Designation,
                    Organization = e.Organization
                }).ToList(),
                Qualifications = qualifications.Select(q => new CandidateQualificationDto
                {
                    FromYear = q.FromYear,
                    ToYear = q.ToYear,
                    Qualification = q.Qualification,
                    BoardUniversity = q.BoardUniversity
                }).ToList()
            };
        }

        public async Task<bool> UpdateCandidateAsync(CandidateDto dto)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(dto.CandidateId);

            if (candidate == null) return false;

            candidate.AppliedDate = dto.AppliedDate.HasValue
                ? DateOnly.FromDateTime(dto.AppliedDate.Value)
                : null;

            candidate.FirstName = dto.FirstName;
            candidate.LastName = dto.LastName;
            candidate.Email = dto.Email;
            candidate.Mobile = dto.Mobile;
            candidate.Gender = dto.Gender;
            candidate.DateOfBirth = dto.DateOfBirth.HasValue
                ? DateOnly.FromDateTime(dto.DateOfBirth.Value)
                : null;
            candidate.MaritalStatus = dto.MaritalStatus;
            candidate.CurrentSalary = dto.CurrentSalary;
            candidate.ExpectedSalary = dto.ExpectedSalary;
            candidate.ReferenceSource = dto.ReferenceSource;
            candidate.Department = dto.Department;
            candidate.Designation = dto.Designation;
            candidate.Skills = dto.Skills;
            candidate.NoticePeriod = dto.NoticePeriod;
            candidate.AnyOffers = dto.AnyOffers;
            candidate.Location = dto.Location;
            candidate.Reason = dto.Reason;

            // 🔹 Update resume only if new file uploaded
            if (!string.IsNullOrEmpty(dto.FileName))
            {
                candidate.FileName = dto.FileName;
                candidate.FilePath = dto.FilePath;
            }

            candidate.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<Candidate>().Update(candidate);

            // 🔹 DELETE OLD EXPERIENCE
            var oldExp = await _unitOfWork.Repository<CandidateExperience>()
                .FindAsync(x => x.CandidateId == dto.CandidateId);
            if (oldExp.Any())
                _unitOfWork.Repository<CandidateExperience>().RemoveRange(oldExp);

            // 🔹 DELETE OLD QUALIFICATIONS
            var oldQual = await _unitOfWork.Repository<CandidateQualification>()
                .FindAsync(x => x.CandidateId == dto.CandidateId);
            if (oldQual.Any())
                _unitOfWork.Repository<CandidateQualification>().RemoveRange(oldQual);

            // 🔹 ADD NEW EXPERIENCE
            if (!string.IsNullOrEmpty(dto.ExperiencesJson))
            {
                var exp = JsonConvert.DeserializeObject<List<CandidateExperienceDto>>(dto.ExperiencesJson);
                await _unitOfWork.Repository<CandidateExperience>()
                    .AddRangeAsync(exp!.Select(e => new CandidateExperience
                    {
                        CandidateId = dto.CandidateId,
                        RegionId = dto.RegionId,
                        CompanyId = dto.CompanyId,
                        UserId = dto.UserId,
                        FromYear = e.FromYear,
                        ToYear = e.ToYear,
                        Designation = e.Designation,
                        Organization = e.Organization,
                        CreatedAt = DateTime.Now,
                        CreatedBy = dto.UserId
                    }));
            }

            // 🔹 ADD NEW QUALIFICATIONS
            if (!string.IsNullOrEmpty(dto.QualificationsJson))
            {
                var qual = JsonConvert.DeserializeObject<List<CandidateQualificationDto>>(dto.QualificationsJson);
                await _unitOfWork.Repository<CandidateQualification>()
                    .AddRangeAsync(qual!.Select(q => new CandidateQualification
                    {
                        CandidateId = dto.CandidateId,
                        RegionId = dto.RegionId,
                        CompanyId = dto.CompanyId,
                        UserId = dto.UserId,
                        FromYear = q.FromYear,
                        ToYear = q.ToYear,
                        Qualification = q.Qualification,
                        BoardUniversity = q.BoardUniversity,
                        CreatedAt = DateTime.Now,
                        CreatedBy = dto.UserId
                    }));
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<object>> GetReferenceUsersAsync()
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(x =>
                     
                    x.Status == "Active");

            return users.Select(u => new
            {
                UserId = u.UserId,
                FullName = u.FullName
            });
        }

       


        /// ////////////////screening///////////////////////

        public async Task<IEnumerable<object>> GetRecruitersAsync()
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(x =>
                   
                   x.RoleId == 1009 &&          // 🔥 ONLY RECRUITERS
                x.Status == "Active");

            return users.Select(u => new
            {
                UserId = u.UserId,
                FullName = u.FullName
            });
        }

        public async Task<IEnumerable<object>> GetScreeningCandidatesTopTableAsync(
int companyId,
int regionId,
string department,
string designation)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(c =>
                    c.CompanyId == companyId &&
                    c.RegionId == regionId &&
                    c.StageId == 2 &&                 // 🔥 ONLY SCREENING
                    c.Department == department &&
                    c.Designation == designation &&
                    c.IsActive
                );

            return candidates.Select(c => new
            {
                c.CandidateId,
                c.SeqNo,
                Name = string.IsNullOrEmpty(c.LastName)
                        ? c.FirstName
                        : $"{c.FirstName} {c.LastName}",
                c.Mobile,
                Expected = c.ExpectedSalary
            });
        }

        public async Task<bool> SaveCandidateScreeningAsync(CandidateScreeningDto dto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var screening = new CandidateScreening
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    CandidateId = dto.CandidateId,
                    RecruiterId = dto.RecruiterId,
                    ScreeningStatus = dto.ScreeningStatus,
                    Remarks = dto.Remarks,
                    ScreeningDate = DateTime.Now,
                    CreatedBy = dto.UserId,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Repository<CandidateScreening>()
                    .AddAsync(screening);

                // 🔥 Move Candidate to INTERVIEW stage
                var candidateRepo = _unitOfWork.Repository<Candidate>();
                var candidate = await candidateRepo.GetByIdAsync(dto.CandidateId);

                if (candidate == null)
                    throw new Exception("Candidate not found");

                candidate.StageId = 3; // Interview
                candidate.ModifiedAt = DateTime.Now;
                candidate.ModifiedBy = dto.UserId;

                candidateRepo.Update(candidate);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<CandidateScreeningDto>> GetScreeningRecordsAsync(int userId,int companyId, int regionId)
        {
            var screenings = await _unitOfWork.Repository<CandidateScreening>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.UserId == userId
                );

            if (!screenings.Any())
                return Enumerable.Empty<CandidateScreeningDto>();

            var candidateIds = screenings.Select(x => x.CandidateId).Distinct().ToList();
            var recruiterIds = screenings.Select(x => x.RecruiterId).Distinct().ToList();

            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => candidateIds.Contains(x.CandidateId));

            var recruiters = await _unitOfWork.Repository<User>()
                .FindAsync(x => recruiterIds.Contains(x.UserId));

            return screenings
                 .OrderByDescending(x => x.CreatedAt)
                 .Select(s =>
                 {
                     var candidate = candidates.First(c => c.CandidateId == s.CandidateId);
                     var recruiter = recruiters.First(r => r.UserId == s.RecruiterId);

                     return new CandidateScreeningDto
                     {
                         CompanyId = s.CompanyId,
                         RegionId = s.RegionId,
                         UserId = s.UserId,
                         CandidateId = s.CandidateId,
                         RecruiterId = s.RecruiterId,
                         ScreeningStatus = s.ScreeningStatus,
                         Remarks = s.Remarks,
                         ScreeningDate = s.ScreeningDate,
                         StageId = candidate.StageId,

                         SeqNo = candidate.SeqNo,
                         CandidateName = string.IsNullOrEmpty(candidate.LastName)
                             ? candidate.FirstName
                             : $"{candidate.FirstName} {candidate.LastName}",
                         RecruiterName = recruiter.FullName,
                         Mobile = candidate.Mobile,
                         ExpectedSalary = candidate.ExpectedSalary
                     };
                 });

        }

        public async Task<bool> UpdateCandidateScreeningAsync(CandidateScreeningDto dto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var screening = (await _unitOfWork.Repository<CandidateScreening>()
                    .FindAsync(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        x.CandidateId == dto.CandidateId))
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                if (screening == null)
                    return false;

                screening.RecruiterId = dto.RecruiterId;
                screening.ScreeningStatus = dto.ScreeningStatus;
                screening.Remarks = dto.Remarks;
                screening.ScreeningDate = DateTime.Now;
                screening.ModifiedAt = DateTime.Now;
                screening.ModifiedBy = dto.UserId;

                _unitOfWork.Repository<CandidateScreening>().Update(screening);

                // 🔥 Update candidate stage based on new result
                var candidateRepo = _unitOfWork.Repository<Candidate>();
                var candidate = await candidateRepo.GetByIdAsync(dto.CandidateId);

                if (candidate != null)
                {
                    if (dto.ScreeningStatus == "Pass")
                        candidate.StageId = 3;   // Interview
                    else
                        candidate.StageId = 2;   // Stay in Screening

                    candidate.ModifiedAt = DateTime.Now;
                    candidate.ModifiedBy = dto.UserId;
                    candidateRepo.Update(candidate);
                }

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }




        /////////////////Interview

        public async Task<IEnumerable<object>> GetScreeningCandidatesTopTableInterviewAsync(
int companyId,
int regionId,
string department,
string designation)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(c =>
                    c.CompanyId == companyId &&
                    c.RegionId == regionId &&
                    c.StageId == 3 &&
                    c.Department == department &&
                    c.Designation == designation &&
                    c.IsActive
                );

            return candidates.Select(c => new
            {
                c.CandidateId,
                c.SeqNo,
                Name = string.IsNullOrEmpty(c.LastName)
                        ? c.FirstName
                        : $"{c.FirstName} {c.LastName}",
                c.Mobile,
                Expected = c.ExpectedSalary
            });
        }
        public async Task<bool> SaveCandidateInterviewAsync(CandidateInterviewDto dto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var interview = new CandidateInterview
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    CandidateId = dto.CandidateId,
                    LevelNo = dto.LevelNo,
                    InterviewerId = dto.InterviewerId,
                    InterviewerName = dto.InterviewerName,
                    InterviewDate = dto.InterviewDate,
                    Location = dto.Location,
                    MeetingLink = dto.MeetingLink,
                    Description = dto.Description,
                    Result = dto.Result ?? "Pending",
                    CreatedAt = DateTime.Now,
                    CreatedBy = dto.UserId
                };

                await _unitOfWork.Repository<CandidateInterview>().AddAsync(interview);

                // 🔄 Update candidate stage
                var candidateRepo = _unitOfWork.Repository<Candidate>();
                var candidate = await candidateRepo.GetByIdAsync(dto.CandidateId);
                if (candidate == null)
                    throw new Exception("Candidate not found");

                candidate.StageId = 4;
                candidate.ModifiedAt = DateTime.Now;
                candidate.ModifiedBy = dto.UserId;
                candidateRepo.Update(candidate);

                // 🔥 FETCH INTERVIEWER
                var interviewer = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(dto.InterviewerId);

                if (interviewer == null || string.IsNullOrEmpty(interviewer.Email))
                    throw new Exception("Interviewer email not found");

                // 🔥 FETCH CANDIDATE EMAIL
                if (string.IsNullOrEmpty(candidate.Email))
                    throw new Exception("Candidate email not found");

                // ================= INTERVIEWER EMAIL =================
                string interviewerSubject = $"Interview Scheduled – {candidate.FirstName} {candidate.LastName}";

                string interviewerBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family:Segoe UI'>
  <h2>Interview Scheduled</h2>
  <p>Dear {interviewer.FullName},</p>

  <p>An interview has been scheduled with the following candidate:</p>

  <table style='border-collapse:collapse'>
    <tr><td><b>Candidate</b></td><td>{candidate.FirstName} {candidate.LastName}</td></tr>
    <tr><td><b>Level</b></td><td>{dto.LevelNo}</td></tr>
    <tr><td><b>Date & Time</b></td><td>{dto.InterviewDate:yyyy-MM-dd HH:mm}</td></tr>
    <tr><td><b>Location</b></td><td>{dto.Location}</td></tr>
    <tr><td><b>Meeting Link</b></td><td>
      {(string.IsNullOrEmpty(dto.MeetingLink) ? "N/A" : $"<a href='{dto.MeetingLink}'>{dto.MeetingLink}</a>")}
    </td></tr>
    <tr><td><b>Description</b></td><td>{dto.Description}</td></tr>
  </table>

  <p>Please be available at the scheduled time.</p>
  <br/>
  <p>Regards,<br/>HR Team</p>
</body>
</html>";

                // ================= CANDIDATE EMAIL =================
                string candidateSubject = "Your Interview Has Been Scheduled";

                string candidateBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family:Segoe UI'>
  <h2>Interview Scheduled</h2>
  <p>Dear {candidate.FirstName},</p>

  <p>Your interview has been scheduled. Please find the details below:</p>

  <table style='border-collapse:collapse'>
    <tr><td><b>Interviewer</b></td><td>{interviewer.FullName}</td></tr>
    <tr><td><b>Level</b></td><td>{dto.LevelNo}</td></tr>
    <tr><td><b>Date & Time</b></td><td>{dto.InterviewDate:yyyy-MM-dd HH:mm}</td></tr>
    <tr><td><b>Location</b></td><td>{dto.Location}</td></tr>
    <tr><td><b>Meeting Link</b></td><td>
      {(string.IsNullOrEmpty(dto.MeetingLink) ? "N/A" : $"<a href='{dto.MeetingLink}'>{dto.MeetingLink}</a>")}
    </td></tr>
  </table>

  <p>Please be available at the scheduled time.</p>
  <br/>
  <p>Best regards,<br/>HR Team</p>
</body>
</html>";

                // 🔥 SEND EMAILS
                await _emailService.SendEmailAsync(interviewer.Email, interviewerSubject, interviewerBody);
                await _emailService.SendEmailAsync(candidate.Email, candidateSubject, candidateBody);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<IEnumerable<CandidateInterviewDto>> GetInterviewRecordsAsync(int userId,
    int companyId,
    int regionId)
        {
            var interviews = await _unitOfWork.Repository<CandidateInterview>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.UserId == userId
                );

            if (!interviews.Any())
                return Enumerable.Empty<CandidateInterviewDto>();

            var candidateIds = interviews.Select(x => x.CandidateId).Distinct().ToList();
            var interviewerIds = interviews.Select(x => x.InterviewerId).Distinct().ToList();

            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => candidateIds.Contains(x.CandidateId));

            var interviewers = await _unitOfWork.Repository<User>()
                .FindAsync(x => interviewerIds.Contains(x.UserId));

            return interviews
     .OrderByDescending(x => x.CreatedAt)
     .Select(iv =>
     {
         var candidate = candidates.First(c => c.CandidateId == iv.CandidateId);
         var interviewer = interviewers.First(u => u.UserId == iv.InterviewerId);

         return new CandidateInterviewDto
         {
             InterviewId = iv.InterviewId,
             CompanyId = iv.CompanyId,
             RegionId = iv.RegionId,
             UserId = iv.UserId,
             CandidateId = iv.CandidateId,
             LevelNo = iv.LevelNo,
             InterviewerId = iv.InterviewerId,
             InterviewerName = interviewer.FullName,
             InterviewDate = iv.InterviewDate,
             Location = iv.Location,
             MeetingLink = iv.MeetingLink,
             Description = iv.Description,
             Result = iv.Result,
             StageId = candidate.StageId,

             SeqNo = candidate.SeqNo,
             CandidateName = string.IsNullOrEmpty(candidate.LastName)
                 ? candidate.FirstName
                 : $"{candidate.FirstName} {candidate.LastName}",
             Mobile = candidate.Mobile,

             // 🔥 ADD THESE
             Department = candidate.Department,
             Designation = candidate.Designation,
             ExpectedSalary = candidate.ExpectedSalary
         };
     });

        }


        public async Task<bool> UpdateCandidateInterviewAsync(CandidateInterviewDto dto)
        {
            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var interviewRepo = _unitOfWork.Repository<CandidateInterview>();

                var interview = await interviewRepo.GetByIdAsync(dto.InterviewId)
                    ?? throw new Exception("Interview record not found");

                interview.Result = dto.Result;
                interview.Description = dto.Description;
                interview.ModifiedAt = DateTime.Now;
                interview.ModifiedBy = dto.UserId;

                interviewRepo.Update(interview);

                // 🔹 Candidate
                var candidateRepo = _unitOfWork.Repository<Candidate>();
                var candidate = await candidateRepo.GetByIdAsync(dto.CandidateId)
                    ?? throw new Exception("Candidate not found");

                // ================= STATUS → STAGE =================
                if (dto.Result == "Selected")
                    candidate.StageId = 5;
                else
                    candidate.StageId = 4;

                candidate.ModifiedAt = DateTime.Now;
                candidate.ModifiedBy = dto.UserId;
                candidateRepo.Update(candidate);

                // ================= EMAILS =================

                // 🔹 HR USERS (RoleId = 4)
                var hrUsers = await _unitOfWork.Repository<User>()
                    .FindAsync(u =>
                        u.RoleId == 4 &&
                        u.CompanyId == interview.CompanyId &&
                        u.RegionId == interview.RegionId &&
                        !string.IsNullOrEmpty(u.Email)
                    );

                if (string.IsNullOrEmpty(candidate.Email))
                    throw new Exception("Candidate email not found");

                // ================= HR EMAIL =================
                string hrSubject = $"Interview Result Updated – {candidate.FirstName} {candidate.LastName}";
                string hrBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family:Segoe UI'>
  <h2>Interview Status Updated</h2>
  <p>Dear HR Team,</p>

  <table>
    <tr><td><b>Candidate</b></td><td>{candidate.FirstName} {candidate.LastName}</td></tr>
    <tr><td><b>Level</b></td><td>{interview.LevelNo}</td></tr>
    <tr><td><b>Status</b></td><td>{dto.Result}</td></tr>
    <tr><td><b>Description</b></td><td>{dto.Description}</td></tr>
  </table>
</body>
</html>";

                // ✅ Send ONE email to ALL HRs (loop is fine but content same)
                foreach (var hr in hrUsers)
                    await _emailService.SendEmailAsync(hr.Email!, hrSubject, hrBody);

                // ================= CANDIDATE EMAIL =================
                string candidateSubject = $"Interview Result – {candidate.FirstName}";
                string candidateBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family:Segoe UI'>
  <h2>Interview Update</h2>
  <p>Dear {candidate.FirstName},</p>

  <p>Your interview result has been updated:</p>

  <table>
    <tr><td><b>Level</b></td><td>{interview.LevelNo}</td></tr>
    <tr><td><b>Status</b></td><td>{dto.Result}</td></tr>
    <tr><td><b>Description</b></td><td>{dto.Description}</td></tr>
  </table>

  <p>Regards,<br/>HR Team</p>
</body>
</html>";

                await _emailService.SendEmailAsync(candidate.Email, candidateSubject, candidateBody);

                // ❌ NO INTERVIEWER EMAIL

                await _unitOfWork.CompleteAsync();
                await tx.CommitAsync();

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }







        ///////////Appointment

        public async Task<IEnumerable<CandidateAppointmentDto>> GetAppointmentsForInterviewerAsync(int interviewerId)
        {
            var interviews = await _unitOfWork.Repository<CandidateInterview>()
                .FindAsync(x =>
                    
                    x.InterviewerId == interviewerId &&
                    x.Result == "Pending"   // 🔥 hide processed ones
                );

            if (!interviews.Any())
                return Enumerable.Empty<CandidateAppointmentDto>();

            var candidateIds = interviews.Select(x => x.CandidateId).Distinct().ToList();

            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => candidateIds.Contains(x.CandidateId));

            return interviews
                .OrderByDescending(x => x.InterviewDate)
                .Select(iv =>
                {
                    var candidate = candidates.FirstOrDefault(c => c.CandidateId == iv.CandidateId);
                    if (candidate == null) return null;

                    return new CandidateAppointmentDto
                    {
                        InterviewId = iv.InterviewId,
                        CandidateId = iv.CandidateId,
                        SeqNo = candidate.SeqNo,
                        InterviewDate = iv.InterviewDate,
                        Designation = candidate.Designation,
                        Location = iv.Location,
                        Description = iv.Description
                    };
                })
                .Where(x => x != null)!;
        }


        public async Task<object?> GetAppointmentCandidateDetailsAsync(int candidateId)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(candidateId);

            if (candidate == null) return null;

            return new
            {
                candidate.CandidateId,
                candidate.SeqNo,
                Name = string.IsNullOrEmpty(candidate.LastName)
                    ? candidate.FirstName
                    : $"{candidate.FirstName} {candidate.LastName}",
                candidate.Gender,
                candidate.Mobile,
                Expected = candidate.ExpectedSalary,
                Status = candidate.StageId,
                DateToJoin = DateTime.Now.AddDays(15)
            };
        }


        ////// Offer

        public async Task<IEnumerable<object>> GetOfferCandidatesTopTableAsync(
int companyId,
int regionId,
string department,
string designation)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(c =>
                    c.CompanyId == companyId &&
                    c.RegionId == regionId &&
                    c.StageId == 5 &&
                    c.Department == department &&
                    c.Designation == designation &&
                    c.IsActive
                );

            return candidates.Select(c => new
            {
                c.CandidateId,
                c.SeqNo,
                Name = string.IsNullOrEmpty(c.LastName)
                        ? c.FirstName
                        : $"{c.FirstName} {c.LastName}",
                c.Mobile,
                Expected = c.ExpectedSalary
            });
        }

        public async Task<bool> SaveCandidateOfferAsync(CandidateOfferDto dto)
        {
            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var offer = new CandidateOffer
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    CandidateId = dto.CandidateId,
                    OfferedCtc = dto.OfferedCtc,
                    ExpectedDoj = DateOnly.FromDateTime(dto.ExpectedDoj),
                    OfferStatus = dto.OfferStatus,
                    Hrname = dto.HrName,
                    OfferLetterPath = dto.OfferLetterPath,
                    FilePath = dto.FilePath,
                    CreatedBy = dto.UserId,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Repository<CandidateOffer>().AddAsync(offer);

                // 🔥 Move candidate to Onboarding stage (Stage = 6)
                var candidateRepo = _unitOfWork.Repository<Candidate>();
                var candidate = await candidateRepo.GetByIdAsync(dto.CandidateId);

                if (candidate == null)
                    throw new Exception("Candidate not found");

                candidate.StageId = 6; // ✅ Onboarding
                candidate.ModifiedAt = DateTime.Now;
                candidate.ModifiedBy = dto.UserId;

                candidateRepo.Update(candidate);

                await _unitOfWork.CompleteAsync();
                await tx.CommitAsync();

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<CandidateOfferDto>> GetOfferRecordsAsync(
    int userId,
    int companyId,
    int regionId)
        {
            var offers = await _unitOfWork.Repository<CandidateOffer>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.UserId == userId
                );

            if (!offers.Any())
                return Enumerable.Empty<CandidateOfferDto>();

            var candidateIds = offers.Select(x => x.CandidateId).Distinct().ToList();

            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => candidateIds.Contains(x.CandidateId));

            return offers
                .OrderByDescending(x => x.CreatedAt)
                .Select(o =>
                {
                    var candidate = candidates.First(c => c.CandidateId == o.CandidateId);

                    return new CandidateOfferDto
                    {
                        OfferId = o.OfferId,
                        CandidateId = o.CandidateId,
                        OfferedCtc = o.OfferedCtc,
                        ExpectedDoj = o.ExpectedDoj.ToDateTime(TimeOnly.MinValue),
                        OfferStatus = o.OfferStatus,
                        HrName = o.Hrname,
                        StageId = candidate.StageId,

                        SeqNo = candidate.SeqNo,
                        CandidateName = string.IsNullOrEmpty(candidate.LastName)
                            ? candidate.FirstName
                            : $"{candidate.FirstName} {candidate.LastName}",
                        Designation = candidate.Designation
                    };
                });
        }

        public async Task<IEnumerable<object>> GetHRUsersAsync(int companyId, int regionId)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.RoleId == 4 &&              // 🔥 HR ROLE
                    x.Status == "Active"
                );

            return users.Select(u => new
            {
                u.UserId,
                u.FullName
            });
        }

        public async Task<bool> SendOfferLetterAsync(int offerId)
        {
            var offerRepo = _unitOfWork.Repository<CandidateOffer>();
            var candidateRepo = _unitOfWork.Repository<Candidate>();

            var offer = await offerRepo.GetByIdAsync(offerId);
            if (offer == null) throw new Exception("Offer not found");

            var candidate = await candidateRepo.GetByIdAsync(offer.CandidateId);
            if (candidate == null) throw new Exception("Candidate not found");

            // ===== FILE SAVE (HTML OFFER LETTER) =====
            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "OfferLetters");
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            string fileName = $"Offer_{candidate.FirstName}_{offer.OfferId}.html";
            string fullPath = Path.Combine(root, fileName);

            string html = $@"
<!DOCTYPE html>
<html>
<body style='font-family:Segoe UI'>
  <h2>Offer Letter</h2>
  <p>Dear {candidate.FirstName},</p>
  <p>We are pleased to offer you the position of <b>{candidate.Designation}</b>.</p>
  <p><b>CTC:</b> {offer.OfferedCtc}</p>
  <p><b>Date of Joining:</b> {offer.ExpectedDoj:dd-MMM-yyyy}</p>
  <p><b>HR:</b> {offer.Hrname}</p>
  <p>Regards,<br/>HR Team</p>
</body>
</html>";

            await File.WriteAllTextAsync(fullPath, html);

            // ===== SAVE PATH =====
            offer.OfferLetterPath = $"Uploads/OfferLetters/{fileName}";
            offerRepo.Update(offer);
            await _unitOfWork.CompleteAsync();

            // ===== EMAIL WITH DOWNLOAD LINK =====
            string downloadUrl = $"https://localhost:44370/{offer.OfferLetterPath}";

            string subject = "Offer Letter – Cortracker HRMS";
            string body = $@"
<p>Dear {candidate.FirstName},</p>
<p>Your offer letter is ready.</p>
<p><a href='{downloadUrl}'>Click here to download your offer letter</a></p>
<p>Regards,<br/>HR Team</p>";

            await _emailService.SendEmailAsync(candidate.Email, subject, body);

            return true;
        }


        public async Task<(byte[] fileBytes, string fileName)> DownloadOfferLetterAsync(int offerId)
        {
            var offer = await _unitOfWork.Repository<CandidateOffer>().GetByIdAsync(offerId);
            if (offer == null || string.IsNullOrEmpty(offer.OfferLetterPath))
                throw new Exception("Offer letter not found");

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", offer.OfferLetterPath);
            var bytes = await File.ReadAllBytesAsync(fullPath);

            return (bytes, Path.GetFileName(fullPath));
        }





        //////////onboarding
        public async Task<IEnumerable<object>> GetOnboardingCandidatesTopTableAsync(
int companyId,
int regionId,
string department,
string designation)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(c =>
                    c.CompanyId == companyId &&
                    c.RegionId == regionId &&
                    c.StageId == 6 &&                 // 🔥 ONLY SCREENING
                    c.Department == department &&
                    c.Designation == designation &&
                    c.IsActive
                );

            return candidates.Select(c => new
            {
                c.CandidateId,
                c.SeqNo,
                Name = string.IsNullOrEmpty(c.LastName)
                        ? c.FirstName
                        : $"{c.FirstName} {c.LastName}",
                c.Mobile,
                Expected = c.ExpectedSalary
            });
        }
        public async Task<int> SaveCandidateOnboardingAsync(CandidateOnboardingDTO dto)
        {
            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var repo = _unitOfWork.Repository<CandidateOnboarding>();

                var existing = (await repo.FindAsync(x =>
                    x.CandidateId == dto.CandidateId &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId))
                    .FirstOrDefault();

                if (existing == null)
                {
                    var entity = new CandidateOnboarding
                    {
                        RegionId = dto.RegionId,
                        CompanyId = dto.CompanyId,
                        UserId = dto.UserId,
                        CandidateId = dto.CandidateId,
                        JoiningDate = dto.JoiningDate.HasValue
                            ? DateOnly.FromDateTime(dto.JoiningDate.Value)
                            : null,
                        DocumentsCollected = dto.DocumentsCollected,
                        BackgroundCheckStatus = dto.BackgroundCheckStatus,
                        LaptopIssued = dto.LaptopIssued,
                        BuddyAssigned = dto.BuddyAssigned,
                        OnboardingStatus = (dto.DocumentsCollected && dto.BackgroundCheckStatus == "Clear")
                            ? "Completed"
                            : "InProgress",
                        CreatedAt = DateTime.Now,
                        CreatedBy = dto.UserId
                    };

                    await repo.AddAsync(entity);
                    await _unitOfWork.CompleteAsync();

                    // ✅ Move candidate to Stage 7 (Onboarding)
                    var candidate = await _unitOfWork.Repository<Candidate>()
                        .GetByIdAsync(dto.CandidateId);

                    if (candidate != null)
                    {
                        candidate.StageId = 7;
                        candidate.ModifiedAt = DateTime.Now;
                        candidate.ModifiedBy = dto.UserId;
                        await _unitOfWork.CompleteAsync();
                    }

                    await tx.CommitAsync();
                    return entity.OnboardingId;
                }
                else
                {
                    existing.JoiningDate = dto.JoiningDate.HasValue
                        ? DateOnly.FromDateTime(dto.JoiningDate.Value)
                        : null;
                    existing.DocumentsCollected = dto.DocumentsCollected;
                    existing.BackgroundCheckStatus = dto.BackgroundCheckStatus;
                    existing.LaptopIssued = dto.LaptopIssued;
                    existing.BuddyAssigned = dto.BuddyAssigned;
                    existing.OnboardingStatus = (dto.DocumentsCollected && dto.BackgroundCheckStatus == "Clear")
                        ? "Completed"
                        : "InProgress";
                    existing.ModifiedAt = DateTime.Now;
                    existing.ModifiedBy = dto.UserId;

                    await _unitOfWork.CompleteAsync();

                    // ✅ Ensure stage is still 7 on update
                    var candidate = await _unitOfWork.Repository<Candidate>()
                        .GetByIdAsync(dto.CandidateId);

                    if (candidate != null && candidate.StageId != 7)
                    {
                        candidate.StageId = 7;
                        candidate.ModifiedAt = DateTime.Now;
                        candidate.ModifiedBy = dto.UserId;
                        await _unitOfWork.CompleteAsync();
                    }

                    await tx.CommitAsync();
                    return existing.OnboardingId;
                }
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<IEnumerable<object>> GetOnboardedCandidatesAsync(int companyId, int regionId)
        {
            var result = await _unitOfWork.Repository<CandidateOnboarding>()
                .FindAsync(x => x.CompanyId == companyId && x.RegionId == regionId);

            var candidateRepo = _unitOfWork.Repository<Candidate>();

            var candidates = await candidateRepo.FindAsync(c =>
                c.CompanyId == companyId &&
                c.RegionId == regionId &&
                c.IsActive);

            return result.Select(o =>
            {
                var cand = candidates.FirstOrDefault(c => c.CandidateId == o.CandidateId);
                return new
                {
                    o.CandidateId,
                    Name = cand != null
                        ? string.IsNullOrEmpty(cand.LastName)
                            ? cand.FirstName
                            : $"{cand.FirstName} {cand.LastName}"
                        : "",
                    o.JoiningDate,
                    DocsCollected = o.DocumentsCollected,
                    BgCheck = o.BackgroundCheckStatus,
                    Laptop = o.LaptopIssued,
                    Buddy = o.BuddyAssigned,
                    Stage = cand?.StageId ?? 0   // ✅ REAL stage (7)
                };
            });
        }


    }
}
