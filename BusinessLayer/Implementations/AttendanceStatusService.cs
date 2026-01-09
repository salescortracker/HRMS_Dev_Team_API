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
    public class AttendanceStatusService : IAttendanceStatusService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceStatusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<AttendanceStatusDto>>> GetAllAsync()
        {
            var list = (await _unitOfWork.Repository<AttendanceStatus>()
                .FindAsync(x => !x.IsDeleted))
                .OrderByDescending(x => x.AttendanceStatusId)
                .Select(x => new AttendanceStatusDto
                {
                    AttendanceStatusID = x.AttendanceStatusId,
                    CompanyID = x.CompanyId,
                    RegionID = x.RegionId,
                    AttendanceStatusName = x.AttendanceStatusName,
                    Description = x.Description,
                    IsActive = x.IsActive
                });

            return new ApiResponse<IEnumerable<AttendanceStatusDto>>(list, "Attendance Status retrieved successfully.");
        }

        public async Task<ApiResponse<AttendanceStatusDto?>> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<AttendanceStatus>().GetByIdAsync(id);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<AttendanceStatusDto?>(null, "Attendance Status not found", false);

            return new ApiResponse<AttendanceStatusDto?>(new AttendanceStatusDto
            {
                AttendanceStatusID = entity.AttendanceStatusId,
                CompanyID = entity.CompanyId,
                RegionID = entity.RegionId,
                AttendanceStatusName = entity.AttendanceStatusName,
                Description = entity.Description,
                IsActive = entity.IsActive
            });
        }

        public async Task<ApiResponse<string>> CreateAsync(CreateUpdateAttendanceStatusDto dto)
        {
            var duplicate = (await _unitOfWork.Repository<AttendanceStatus>()
                .FindAsync(x =>
                    !x.IsDeleted &&
                    x.CompanyId == dto.CompanyID &&
                    x.RegionId == dto.RegionID &&
                    x.AttendanceStatusName.ToLower() == dto.AttendanceStatusName.ToLower()))
                .Any();

            if (duplicate)
                return new ApiResponse<string>(null!, "Duplicate Attendance Status exists.", false);

            await _unitOfWork.Repository<AttendanceStatus>().AddAsync(new AttendanceStatus
            {
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                AttendanceStatusName = dto.AttendanceStatusName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.CompleteAsync();
            return new ApiResponse<string>("Attendance Status created successfully.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(CreateUpdateAttendanceStatusDto dto)
        {
            var entity = await _unitOfWork.Repository<AttendanceStatus>().GetByIdAsync(dto.AttendanceStatusID);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<string>(null!, "Attendance Status not found.", false);

            entity.AttendanceStatusName = dto.AttendanceStatusName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;

            // ✅ Update company and region
            entity.CompanyId = dto.CompanyID;
            entity.RegionId = dto.RegionID;

            entity.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<AttendanceStatus>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Attendance Status updated successfully.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<AttendanceStatus>().GetByIdAsync(id);

            if (entity == null)
                return new ApiResponse<string>(null!, "Attendance Status not found.", false);

            _unitOfWork.Repository<AttendanceStatus>().Delete(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Attendance Status deleted successfully.");
        }
    }
}
