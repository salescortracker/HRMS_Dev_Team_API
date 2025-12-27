using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class RecruitmentService : IRecruitmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecruitmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ---------------- Add Candidate ----------------
        public async Task<int> AddCandidateAsync(CandidateCreateDto dto)
        {
            var existing = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => x.Email == dto.Email);

            if (existing.Any())
                throw new Exception("Candidate email already exists");

            var entity = new Candidate
            {
                RegionId = dto.RegionId,
                CompanyId = dto.CompanyId,
                UserId = dto.UserId,

                CandidateName = dto.CandidateName,
                Email = dto.Email,
                Mobile = dto.Mobile,
                Technology = dto.Technology,
                ExperienceYears = dto.ExperienceYears,
                CurrentCtc = dto.CurrentCTC,

                FileName = dto.FileName!,
                FilePath = dto.FilePath,
                AppliedDate = DateOnly.FromDateTime(dto.AppliedDate),

                CurrentStageId = 1,
                ProgressPercent = 0,

                CreatedBy = dto.UserId,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<Candidate>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            await _unitOfWork.Repository<CandidateStageHistory>().AddAsync(
                new CandidateStageHistory
                {
                    CandidateId = entity.CandidateId,
                    StageId = 1,
                    ChangedBy = dto.UserId,
                    Remarks = "Candidate Added"
                });

            await _unitOfWork.CompleteAsync();

            return entity.CandidateId;
        }


        // ---------------- Listing ----------------
        public async Task<List<CandidateListDto>> GetCandidatesAsync(int userId, int roleId)
        {
            var candidates = await _unitOfWork.Repository<Candidate>().GetAllAsync();
            var stages = await _unitOfWork.Repository<RecruitmentStage>().GetAllAsync();

            // 🔥 Filter by logged-in user always
            candidates = candidates.Where(x => x.UserId == userId);

            var result =
                from c in candidates
                join s in stages on c.CurrentStageId equals s.StageId
                orderby c.CreatedAt descending
                select new CandidateListDto
                {
                    CandidateId = c.CandidateId,
                    CandidateName = c.CandidateName,
                    Email = c.Email,
                    Technology = c.Technology,
                    ExperienceYears = c.ExperienceYears,
                    AppliedDate = c.AppliedDate.ToDateTime(TimeOnly.MinValue),
                    StageName = s.StageName,
                    ProgressPercent = c.ProgressPercent,
                    FileName = c.FileName,
                    FilePath = c.FilePath
                };

            return result.ToList();
        }


        // ---------------- Advance Stage ----------------
        public async Task AdvanceStageAsync(int candidateId, int userId)
        {
            var candidateList = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => x.CandidateId == candidateId);

            var candidate = candidateList.FirstOrDefault();
            if (candidate == null)
                throw new Exception("Candidate not found");

            var stages = await _unitOfWork.Repository<RecruitmentStage>().GetAllAsync();

            var nextStage = stages
                .FirstOrDefault(x => x.StageId == candidate.CurrentStageId + 1);

            if (nextStage == null) return;

            candidate.CurrentStageId = nextStage.StageId;
            candidate.ProgressPercent = nextStage.ProgressPercent;
            candidate.ModifiedBy = userId;
            candidate.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<Candidate>().Update(candidate);
            await _unitOfWork.CompleteAsync();

            await _unitOfWork.Repository<CandidateStageHistory>().AddAsync(
                new CandidateStageHistory
                {
                    CandidateId = candidateId,
                    StageId = nextStage.StageId,
                    ChangedBy = userId,
                    Remarks = "Advanced Stage"
                });

            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteCandidateAsync(int candidateId, int userId)
        {
            // 1. Get candidate
            var candidate = (await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => x.CandidateId == candidateId))
                .FirstOrDefault();

            if (candidate == null)
                throw new Exception("Candidate not found");

            // 2. Get dependent records (Stage History)
            var histories = await _unitOfWork.Repository<CandidateStageHistory>()
                .FindAsync(x => x.CandidateId == candidateId);

            // 3. Delete child records FIRST
            if (histories.Any())
            {
                _unitOfWork.Repository<CandidateStageHistory>()
                    .RemoveRange(histories);
            }

            // 4. Delete candidate
            _unitOfWork.Repository<Candidate>().Remove(candidate);

            // 5. Save changes
            await _unitOfWork.CompleteAsync();
        }
        public async Task<IEnumerable<RecruiterDto>> GetRecruitersAsync()
        {
            var users = await _unitOfWork.Repository<User>().GetAllAsync();

            return users
                .Where(u => u.RoleId == 1009)   // ✅ Recruiter role only
                .Select(u => new RecruiterDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName
                    
                })
                .ToList();
        }

        public async Task<List<CandidateListDto>> GetScreeningCandidatesAsync(int userId, int roleId)
        {
            var allScreened = await _unitOfWork.Repository<CandidateScreening>().GetAllAsync();
            var screenedIds = allScreened.Select(x => x.CandidateId).ToList();

            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(c => c.CurrentStageId == 2 && !screenedIds.Contains(c.CandidateId));

            var stages = await _unitOfWork.Repository<RecruitmentStage>().GetAllAsync();

            if (roleId != 4) // HR only
                candidates = candidates.Where(c => c.UserId == userId);

            return (
                from c in candidates
                join s in stages on c.CurrentStageId equals s.StageId
                select new CandidateListDto
                {
                    CandidateId = c.CandidateId,
                    CandidateName = c.CandidateName,
                    Email = c.Email,
                    Technology = c.Technology,
                    ExperienceYears = c.ExperienceYears,
                    AppliedDate = c.AppliedDate.ToDateTime(TimeOnly.MinValue),
                    StageName = s.StageName,
                    ProgressPercent = c.ProgressPercent
                }
            ).ToList();
        }


        public async Task SaveCandidateScreeningAsync(CandidateScreeningCreateDto dto)
        {
            foreach (var candidateId in dto.CandidateIds)
            {
                var candidate = (await _unitOfWork.Repository<Candidate>()
                    .FindAsync(c => c.CandidateId == candidateId))
                    .FirstOrDefault();

                if (candidate == null) continue;

                // ✔️ Update only if PASS
                if (dto.Result == "Pass")
                {
                    candidate.CurrentStageId = 3; // Interview
                    candidate.ProgressPercent = 60; // Increase stage
                }
                else
                {
                    // ❌ Do NOT update stage for Hold / Reject
                    // ❌ Do NOT touch progress
                    candidate.CurrentStageId = 2; // Stay in Screening
                }

                candidate.ModifiedBy = dto.UserId;
                candidate.ModifiedAt = DateTime.Now;
                _unitOfWork.Repository<Candidate>().Update(candidate);

                // Save Screening Record
                var screening = new CandidateScreening
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    CandidateId = candidateId,
                    RecruiterIds = string.Join(",", dto.RecruiterIds),
                    Result = dto.Result,
                    Remarks = dto.Remarks,
                    CreatedBy = dto.UserId,
                    CreatedAt = DateTime.Now
                };
                await _unitOfWork.Repository<CandidateScreening>().AddAsync(screening);

                // Add Stage History
                await _unitOfWork.Repository<CandidateStageHistory>().AddAsync(
                    new CandidateStageHistory
                    {
                        CandidateId = candidateId,
                        StageId = candidate.CurrentStageId,
                        ChangedBy = dto.UserId,
                        Remarks = $"{dto.Result} - {dto.Remarks}"
                    });
            }

            await _unitOfWork.CompleteAsync();
        }



        public async Task<List<CandidateScreeningListDto>> GetScreeningRecordsAsync(
    int companyId,
    int regionId)
        {
            var screenings = await _unitOfWork.Repository<CandidateScreening>()
                .FindAsync(s => s.CompanyId == companyId && s.RegionId == regionId);

            var candidates = await _unitOfWork.Repository<Candidate>().GetAllAsync();

            var result =
                from s in screenings
                join c in candidates on s.CandidateId equals c.CandidateId
                orderby s.CreatedAt descending
                select new CandidateScreeningListDto
                {
                    ScreeningId = s.ScreeningId,
                    CandidateName = c.CandidateName,
                    Recruiters = s.RecruiterIds,
                    Result = s.Result,
                    Remarks = s.Remarks,
                    CreatedAt = s.CreatedAt,
                    ProgressPercent = c.ProgressPercent
                };

            return result.ToList();
        }


    }
}
