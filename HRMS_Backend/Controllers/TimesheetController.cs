using System.Text.Json;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheetService _timesheetService;
        public TimesheetController(ITimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }
        [HttpGet("GetLoggedInUser/{userId}")]
        public async Task<IActionResult> GetLoggedInUser(int userId)
        {
            var data = await _timesheetService.GetLoggedInUserAsync(userId);
            return Ok(data);
        }
        [HttpPost("SaveTimesheet")]
        public async Task<IActionResult> SaveTimesheet([FromForm] TimesheetRequestDto dto)
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = Path.Combine(root, "Uploads", "Timesheets");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // 📎 File upload
            if (dto.Attachment != null && dto.Attachment.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{dto.Attachment.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await dto.Attachment.CopyToAsync(stream);

                dto.FileName = fileName;
                dto.FilePath = $"Uploads/Timesheets/{fileName}";
            }

            int id = await _timesheetService.SaveTimesheetAsync(dto);
            return Ok(new { message = "Timesheet saved successfully", timesheetId = id });
        }

        // ✅ USER LISTING
        [HttpGet("GetMyTimesheets/{userId}")]
        public async Task<IActionResult> GetMyTimesheets(int userId)
        {
            var data = await _timesheetService.GetMyTimesheetsAsync(userId);
            return Ok(data);
        }


    }
}
