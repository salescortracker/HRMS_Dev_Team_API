using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class MissedPunchService : IMissedPunchService
    {
        private readonly HRMSContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public MissedPunchService(HRMSContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        // ================================
        // 1. Get My Requests (Employee)
        // ================================
        public async Task<List<MissedPunchRequestDto>> GetMyRequestsAsync(int employeeId)
        {
            return await _context.MissedPunchRequests
                .Include(r => r.MissedTypeNavigation)
                .Where(r => r.EmployeeId == employeeId)
                .Select(r => new MissedPunchRequestDto
                {
                    MissedPunchRequestID = r.MissedPunchRequestId,
                    EmployeeID = r.EmployeeId,
                    ManagerID = r.ManagerId ?? 0,
                    MissedDate = r.MissedDate.ToDateTime(TimeOnly.MinValue),
                    MissedTypeID = r.MissedTypeId ?? 0,
                    MissedType = r.MissedTypeNavigation != null ? r.MissedTypeNavigation.MissedType1 : (r.MissedType ?? "Unknown"),
                    CorrectClockIn = r.CorrectClockIn.HasValue ? DateTime.Today.Add(r.CorrectClockIn.Value.ToTimeSpan()) : null,
                    CorrectClockOut = r.CorrectClockOut.HasValue ? DateTime.Today.Add(r.CorrectClockOut.Value.ToTimeSpan()) : null,
                    Reason = r.Reason,
                    ManagerRemarks = r.ManagerRemarks,
                    Status = r.Status,
                    CompanyID = r.CompanyId,
                    RegionID = r.RegionId
                })
                .ToListAsync();
        }

        // ===================================
        // 2. Get Pending Requests (Manager)
        // ===================================
        public async Task<List<MissedPunchRequestDto>> GetPendingRequestsForManagerAsync(int managerId)
        {
            return await _context.MissedPunchRequests
                .Include(r => r.MissedTypeNavigation)
                .Where(r => r.ManagerId == managerId && r.Status == "Pending")
                .Select(r => new MissedPunchRequestDto
                {
                    MissedPunchRequestID = r.MissedPunchRequestId,
                    EmployeeID = r.EmployeeId,
                    ManagerID = r.ManagerId ?? 0,
                    MissedDate = r.MissedDate.ToDateTime(TimeOnly.MinValue),
                    MissedTypeID = r.MissedTypeId ?? 0,
                    MissedType = r.MissedTypeNavigation != null ? r.MissedTypeNavigation.MissedType1 : (r.MissedType ?? "Unknown"),
                    CorrectClockIn = r.CorrectClockIn.HasValue ? DateTime.Today.Add(r.CorrectClockIn.Value.ToTimeSpan()) : null,
                    CorrectClockOut = r.CorrectClockOut.HasValue ? DateTime.Today.Add(r.CorrectClockOut.Value.ToTimeSpan()) : null,
                    Reason = r.Reason,
                    ManagerRemarks = r.ManagerRemarks,
                    Status = r.Status,
                    CompanyID = r.CompanyId,
                    RegionID = r.RegionId
                })
                .ToListAsync();
        }

        // ================================
        // 3. Submit Request
        // ================================
        public async Task<MissedPunchRequestDto> SubmitRequestAsync(MissedPunchRequestDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.EmployeeID);
            if (user == null)
                throw new InvalidOperationException("Employee not found.");

            if (!user.ReportingTo.HasValue || user.ReportingTo.Value == 0)
                throw new InvalidOperationException("No reporting manager assigned. Please contact HR.");

            dto.CompanyID = user.CompanyId;
            dto.RegionID = user.RegionId;
            dto.ManagerID = user.ReportingTo.Value;

            var missedTypeEntity = await _context.MissedTypes.FirstOrDefaultAsync(m => m.MissedTypeId == dto.MissedTypeID);
            if (missedTypeEntity == null)
                throw new InvalidOperationException($"MissedTypeID {dto.MissedTypeID} is invalid.");

            var entity = new MissedPunchRequest
            {
                EmployeeId = dto.EmployeeID,
                ManagerId = dto.ManagerID,
                MissedDate = DateOnly.FromDateTime(dto.MissedDate),
                MissedTypeId = dto.MissedTypeID,
                MissedType = missedTypeEntity.MissedType1,
                CorrectClockIn = dto.CorrectClockIn.HasValue ? TimeOnly.FromDateTime(dto.CorrectClockIn.Value) : null,
                CorrectClockOut = dto.CorrectClockOut.HasValue ? TimeOnly.FromDateTime(dto.CorrectClockOut.Value) : null,
                Reason = dto.Reason,
                Status = "Pending",
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                UserId = dto.EmployeeID,
                CreatedAt = DateTime.Now,
                CreatedBy = dto.EmployeeID
            };

            _context.MissedPunchRequests.Add(entity);
            await _context.SaveChangesAsync();

            dto.MissedPunchRequestID = entity.MissedPunchRequestId;
            dto.Status = entity.Status;
            dto.MissedType = missedTypeEntity.MissedType1;

            // Send email to manager
            if (dto.ManagerID != 0)
            {
                var manager = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.ManagerID);
                if (manager != null)
                {
                    string subject = $"Missed Punch Request from Employee {dto.EmployeeID}";
                    string body = $@"
                        <p>Employee ID: {dto.EmployeeID}</p>
                        <p>Missed Date: {dto.MissedDate:yyyy-MM-dd}</p>
                        <p>Missed Type: {dto.MissedType}</p>
                        <p>Clock In: {(dto.CorrectClockIn.HasValue ? dto.CorrectClockIn.Value.ToString("HH:mm") : "-")}</p>
                        <p>Clock Out: {(dto.CorrectClockOut.HasValue ? dto.CorrectClockOut.Value.ToString("HH:mm") : "-")}</p>
                        <p>Reason: {dto.Reason}</p>
                        <br/>
                        <p>
                            <a href='https://frontend/manager/missedpunch/approve?requestId={dto.MissedPunchRequestID}'>Approve</a> | 
                            <a href='https://frontend/manager/missedpunch/reject?requestId={dto.MissedPunchRequestID}'>Reject</a>
                        </p>
                    ";

                    await _emailService.SendEmailAsync(manager.Email, subject, body);
                }
            }

            return dto;
        }

        // ================================
        // 4. Approve / Reject Request
        // ================================
        public async Task<bool> TakeActionAsync(MissedPunchActionDto actionDto)
        {
            var request = await _context.MissedPunchRequests
                .FirstOrDefaultAsync(r => r.MissedPunchRequestId == actionDto.RequestId);

            if (request == null) return false;

            if (request.ManagerId != actionDto.ManagerId)
                throw new InvalidOperationException("You are not authorized to approve/reject this request.");

            request.Status = actionDto.Status;
            request.ManagerRemarks = actionDto.ManagerRemarks;
            request.ModifiedAt = DateTime.Now;
            request.ModifiedBy = actionDto.ManagerId;

            await _context.SaveChangesAsync();

            var employee = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.EmployeeId);
            if (employee != null)
            {
                string empSubject = $"Your Missed Punch Request has been {request.Status}";
                string empBody = $@"
                    <p>Dear Employee,</p>
                    <p>Your missed punch request for <b>{request.MissedDate:yyyy-MM-dd}</b> has been <b>{request.Status}</b> by your manager.</p>
                    <p><b>Manager Remarks:</b> {request.ManagerRemarks}</p>
                    <br/>
                    <p>Thank you.</p>
                ";
                await _emailService.SendEmailAsync(employee.Email, empSubject, empBody);
            }

            return true;
        }

        // ================================
        // 5. Get Active Missed Types
        // ================================
        public async Task<List<MissedTypeDto>> GetActiveMissedTypesAsync()
        {
            return await _context.MissedTypes
                .Where(t => t.IsActive && !t.IsDeleted)
                .Select(t => new MissedTypeDto
                {
                    MissedTypeID = t.MissedTypeId,
                    MissedType = t.MissedType1
                })
                .ToListAsync();
        }
    }
}
