using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class MissedPunchService : IMissedPunchService
    {
        private readonly HRMSContext _context;

        public MissedPunchService(HRMSContext context)
        {
            _context = context;
        }

        // ================================
        // 1. Get My Requests
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
                    MissedType = r.MissedTypeNavigation != null ? r.MissedTypeNavigation.MissedType1 : r.MissedType,

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
        // 2. Get Pending Requests (Manager)
        // ================================
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
                    MissedType = r.MissedTypeNavigation != null ? r.MissedTypeNavigation.MissedType1 : r.MissedType,

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
            // 🔥 IMPORTANT FIX — fetch MissedType from DB using ID
            var missedTypeEntity = await _context.MissedTypes
                .FirstOrDefaultAsync(m => m.MissedTypeId == dto.MissedTypeID);

            if (missedTypeEntity == null)
            {
                // Return a friendly error instead of crashing
                throw new InvalidOperationException($"MissedTypeID {dto.MissedTypeID} is invalid or does not exist.");
            }

            var entity = new MissedPunchRequest
            {
                EmployeeId = dto.EmployeeID,
                ManagerId = dto.ManagerID != 0 ? dto.ManagerID : null,

                MissedDate = DateOnly.FromDateTime(dto.MissedDate),

                MissedTypeId = dto.MissedTypeID,
                MissedType = missedTypeEntity.MissedType1,   // ✅ CORRECT — satisfies CHECK constraint

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

            // Update the DTO to return
            dto.MissedPunchRequestID = entity.MissedPunchRequestId;
            dto.Status = entity.Status;
            dto.MissedType = missedTypeEntity.MissedType1;

            return dto;
        }


        // ================================
        // 4. Approve / Reject
        // ================================
        public async Task<bool> TakeActionAsync(MissedPunchActionDto actionDto)
        {
            var request = await _context.MissedPunchRequests
                .FirstOrDefaultAsync(r => r.MissedPunchRequestId == actionDto.RequestId);

            if (request == null)
                return false;

            request.Status = actionDto.Status;
            request.ManagerRemarks = actionDto.ManagerRemarks;

            request.ModifiedAt = DateTime.Now;
            request.ModifiedBy = actionDto.ManagerId;

            await _context.SaveChangesAsync();
            return true;
        }

        // ================================
        // 5. Get Missed Types
        // ================================
        public async Task<List<MissedTypeDto>> GetActiveMissedTypesAsync()
        {
            return await _context.MissedTypes
                .Where(t => t.IsActive && !t.IsDeleted)
                .Select(t => new MissedTypeDto
                {
                    MissedTypeID = t.MissedTypeId,
                    MissedType = t.MissedType1
                }).ToListAsync();
        }
    }
}