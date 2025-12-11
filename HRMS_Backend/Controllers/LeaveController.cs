using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;
       
        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
           
        }
        [HttpGet("GetReportingManager/{userId}")]
        public async Task<IActionResult> GetReportingManager(int userId)
        {
            var data = await _leaveService.GetReportingManagerAsync(userId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetActiveLeaveTypes")]
        public async Task<IActionResult> GetActiveLeaveTypes()
        {
            var data = await _leaveService.GetActiveLeaveTypesAsync();
            return Ok(data);
        }
        [HttpPost("SubmitLeave")]
        public async Task<IActionResult> SubmitLeave([FromForm] LeaveRequestDto dto)
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = Path.Combine(root, "Uploads", "LeaveDocuments");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // ✅ File upload
            if (dto.SupportingDocument != null && dto.SupportingDocument.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{dto.SupportingDocument.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await dto.SupportingDocument.CopyToAsync(stream);

                dto.FileName = fileName;
                dto.FilePath = $"Uploads/LeaveDocuments/{fileName}";
            }

            // ✅ Save Leave
            int leaveId = await _leaveService.SubmitLeaveAsync(dto);

            // ✅ Send Email to Manager
            await _leaveService.SendLeaveEmailToManagerAsync(leaveId);

            return Ok(new { message = "Leave submitted and email sent successfully", id = leaveId });
        }

        [HttpGet("GetMyLeaves/{userId}")]
        public async Task<IActionResult> GetMyLeaves(int userId)
        {
            var data = await _leaveService.GetMyLeavesAsync(userId);
            return Ok(data);
        }


        [HttpGet("ApproveFromEmail/{leaveId}")]
        public async Task<IActionResult> ApproveFromEmail(int leaveId)
        {
            var result = await _leaveService.ApproveLeaveFromEmailAsync(leaveId);
            if (!result) return Content("Leave request not found");

            return Content("<h2>✅ Leave Approved Successfully</h2>", "text/html");
        }

        [HttpGet("RejectFromEmail/{leaveId}")]
        public async Task<IActionResult> RejectFromEmail(int leaveId)
        {
            var result = await _leaveService.RejectLeaveFromEmailAsync(leaveId);
            if (!result) return Content("Leave request not found");

            return Content("<h2>❌ Leave Rejected Successfully</h2>", "text/html");
        }
        [HttpGet("GetLeavesForManager/{managerId}")]
        public async Task<IActionResult> GetLeavesForManager(int managerId)
        {
            var data = await _leaveService.GetLeavesForManagerAsync(managerId);
            return Ok(data);
        }

        [HttpPost("ApproveByManager/{leaveId}")]
        public async Task<IActionResult> ApproveByManager(int leaveId)
        {
            var result = await _leaveService.ApproveLeaveByManagerAsync(leaveId);
            return result ? Ok("Approved") : BadRequest("Failed");
        }

        [HttpPost("RejectByManager/{leaveId}")]
        public async Task<IActionResult> RejectByManager(int leaveId)
        {
            var result = await _leaveService.RejectLeaveByManagerAsync(leaveId);
            return result ? Ok("Rejected") : BadRequest("Failed");
        }

        [HttpPost("BulkApprove")]
        public async Task<IActionResult> BulkApprove([FromBody] List<int> leaveIds)
        {
            var result = await _leaveService.BulkApproveLeavesAsync(leaveIds);
            return result ? Ok("Bulk Approved") : BadRequest();
        }

        [HttpPost("BulkReject")]
        public async Task<IActionResult> BulkReject([FromBody] List<int> leaveIds)
        {
            var result = await _leaveService.BulkRejectLeavesAsync(leaveIds);
            return result ? Ok("Bulk Rejected") : BadRequest();
        }
        [HttpGet("GetUserLeaves/{userId}")]
        public async Task<IActionResult> GetUserLeaves(int userId)
        {
            var result = await _leaveService.GetLeavesForUserAsync(userId);
            return Ok(result);
        }

        [HttpGet("GetManagerLeaves/{managerId}")]
        public async Task<IActionResult> GetManagerLeaves(int managerId)
        {
            var result = await _leaveService.GetLeavesForManagerAsync(managerId);
            return Ok(result);
        }
    }
}
