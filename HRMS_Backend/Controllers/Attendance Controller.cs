using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IClockInOutService _clockInOutService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(IClockInOutService clockInOutService, ILogger<AttendanceController> logger)
        {
            _clockInOutService = clockInOutService;
            _logger = logger;
        }

        [HttpGet("GetClockInOutAll")]
        public async Task<IActionResult> GetClockInOutAll()
        {
            return Ok(await _clockInOutService.GetAllAsync());
        }

        [HttpGet("TodayByEmployee")]
        public async Task<IActionResult> GetTodayByEmployee(string employeeCode, int companyId, int regionId)
        {
            var data = await _clockInOutService.GetTodayByEmployeeAsync(employeeCode, companyId, regionId);
            return Ok(data);
        }

        [HttpPost("CreateClockInOut")]
        public async Task<IActionResult> CreateClockInOut([FromBody] ClockInOutCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = 1; // replace with actual user id
            var result = await _clockInOutService.AddAsync(dto, userId);
            return Ok(result);
        }

        [HttpPost("DeleteClockInOut/{id}")]
        public async Task<IActionResult> DeleteClockInOut(int id)
        {
            int userId = 1;
            bool success = await _clockInOutService.DeleteAsync(id, userId);
            if (!success) return NotFound("Attendance record not found");
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
