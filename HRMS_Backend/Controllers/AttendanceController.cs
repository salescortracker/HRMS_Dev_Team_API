using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ILateArrivalService _lateArrivalService;

        public AttendanceController(ILateArrivalService lateArrivalService)
        {
            _lateArrivalService = lateArrivalService;
        }


        [HttpGet("late-arrivals")]
        public async Task<IActionResult> GetLateArrivals(
          int companyId,
          int regionId,
          DateOnly fromDate,
          DateOnly toDate,
          string? employeeCode = null)
        {
            var result = await _lateArrivalService.GetLateArrivalsAsync(
                companyId, regionId, fromDate, toDate, employeeCode);

            return Ok(result);
        }
    }
}
