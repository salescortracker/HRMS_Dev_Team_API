using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruitmenController : ControllerBase
    {
        private readonly IRecruitmentService _service;

        public RecruitmenController(IRecruitmentService service)
        {
            _service = service;
        }

        // ---------------- Add Candidate ----------------
        [HttpPost("AddCandidate")]
        public async Task<IActionResult> AddCandidate([FromForm] CandidateCreateDto dto)
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = Path.Combine(root, "Uploads", "Resumes");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{dto.ResumeFile.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await dto.ResumeFile.CopyToAsync(stream);

                dto.FileName = fileName;
                dto.FilePath = $"Uploads/Resumes/{fileName}";
            }

            int id = await _service.AddCandidateAsync(dto);
            return Ok(new { message = "Candidate added successfully", id });
        }

        // ---------------- Listing ----------------
        [HttpGet("GetCandidates/{userId}/{roleId}")]
        public async Task<IActionResult> GetCandidates(int userId, int roleId)
        {
            var data = await _service.GetCandidatesAsync(userId, roleId);
            return Ok(data);
        }

        // ---------------- Advance ----------------
        [HttpPut("AdvanceStage/{candidateId}/{userId}")]
        public async Task<IActionResult> AdvanceStage(int candidateId, int userId)
        {
            await _service.AdvanceStageAsync(candidateId, userId);
            return Ok(new { message = "Stage advanced successfully" });
        }
        [HttpDelete("DeleteCandidate/{candidateId}/{userId}")]
        public async Task<IActionResult> DeleteCandidate(int candidateId, int userId)
        {
            await _service.DeleteCandidateAsync(candidateId, userId);
            return Ok(new { message = "Candidate deleted successfully" });
        }
        [HttpGet]
        [Route("GetRecruiters")]
        public async Task<IActionResult> GetRecruiters()
        {
            var data = await _service.GetRecruitersAsync();
            return Ok(data);
        }
        [HttpGet("GetScreeningCandidates/{userId}/{roleId}")]
        public async Task<IActionResult> GetScreeningCandidates(int userId, int roleId)
        {
            var data = await _service.GetScreeningCandidatesAsync(userId, roleId);
            return Ok(data);
        }

        [HttpPost("SaveScreening")]
        public async Task<IActionResult> SaveScreening(
     CandidateScreeningCreateDto dto)
        {
            await _service.SaveCandidateScreeningAsync(dto);
            return Ok(new { message = "Screening saved successfully" });
        }


        [HttpGet("GetScreeningRecords/{companyId}/{regionId}")]
        public async Task<IActionResult> GetScreeningRecords(
    int companyId,
    int regionId, int userId)
        {
            var data = await _service.GetScreeningRecordsAsync(companyId, regionId);
            return Ok(data);
        }


    }
}
