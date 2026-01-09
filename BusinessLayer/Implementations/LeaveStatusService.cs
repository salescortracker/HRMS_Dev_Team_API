using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class LeaveStatusService : ILeaveStatusService
    {
       
            private readonly IUnitOfWork _unitOfWork;

            public LeaveStatusService(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<ApiResponse<IEnumerable<LeaveStatusDto>>> GetAllAsync()
            {
                var list = (await _unitOfWork.Repository<LeaveStatus>()
                    .FindAsync(x => !x.IsDeleted))
                    .OrderByDescending(x => x.LeaveStatusId)
                    .Select(x => new LeaveStatusDto
                    {
                        LeaveStatusID = x.LeaveStatusId,
                        CompanyID = x.CompanyId,
                        RegionID = x.RegionId,
                        LeaveStatusName = x.LeaveStatusName,
                        Description = x.Description,
                        IsActive = x.IsActive
                    });

                return new ApiResponse<IEnumerable<LeaveStatusDto>>(list, "Leave Status retrieved successfully.");
            }

            public async Task<ApiResponse<LeaveStatusDto?>> GetByIdAsync(int id)
            {
                var entity = await _unitOfWork.Repository<LeaveStatus>().GetByIdAsync(id);

                if (entity == null || entity.IsDeleted)
                    return new ApiResponse<LeaveStatusDto?>(null, "Leave Status not found", false);

                return new ApiResponse<LeaveStatusDto?>(new LeaveStatusDto
                {
                    LeaveStatusID = entity.LeaveStatusId,
                    CompanyID = entity.CompanyId,
                    RegionID = entity.RegionId,
                    LeaveStatusName = entity.LeaveStatusName,
                    Description = entity.Description,
                    IsActive = entity.IsActive
                });
            }

            public async Task<ApiResponse<string>> CreateAsync(CreateUpdateLeaveStatusDto dto)
            {
                var duplicate = (await _unitOfWork.Repository<LeaveStatus>().FindAsync(x =>
                    !x.IsDeleted &&
                    x.CompanyId == dto.CompanyID &&
                    x.RegionId == dto.RegionID &&
                    x.LeaveStatusName.ToLower() == dto.LeaveStatusName.ToLower()))
                    .Any();

                if (duplicate)
                    return new ApiResponse<string>(null!, "Duplicate Leave Status exists.", false);

                await _unitOfWork.Repository<LeaveStatus>().AddAsync(new LeaveStatus
                {
                    CompanyId = dto.CompanyID,
                    RegionId = dto.RegionID,
                    LeaveStatusName = dto.LeaveStatusName,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });

                await _unitOfWork.CompleteAsync();
                return new ApiResponse<string>("Leave Status created successfully.");
            }

            public async Task<ApiResponse<string>> UpdateAsync(CreateUpdateLeaveStatusDto dto)
            {
                var entity = await _unitOfWork.Repository<LeaveStatus>().GetByIdAsync(dto.LeaveStatusID);

                if (entity == null || entity.IsDeleted)
                    return new ApiResponse<string>(null!, "Leave Status not found.", false);

                entity.LeaveStatusName = dto.LeaveStatusName;
                entity.Description = dto.Description;
                entity.IsActive = dto.IsActive;
                entity.ModifiedAt = DateTime.UtcNow;

                _unitOfWork.Repository<LeaveStatus>().Update(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>("Leave Status updated successfully.");
            }

            public async Task<ApiResponse<string>> DeleteAsync(int id)
            {
                var entity = await _unitOfWork.Repository<LeaveStatus>().GetByIdAsync(id);

                if (entity == null)
                    return new ApiResponse<string>(null!, "Leave Status not found.", false);

                _unitOfWork.Repository<LeaveStatus>().Delete(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>("Leave Status deleted successfully.");
            }
        }

    }
