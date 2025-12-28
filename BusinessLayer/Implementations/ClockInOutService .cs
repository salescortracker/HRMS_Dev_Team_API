using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class ClockInOutService : IClockInOutService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClockInOutService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ClockInOutDto>> GetAllAsync()
        {
            var data = await _unitOfWork.Repository<ClockInOut>().GetAllAsync();

            return data
                .OrderByDescending(x => x.AttendanceDate)
                .ThenByDescending(x => x.ActionTime)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<ClockInOutDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<ClockInOut>().GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<IEnumerable<ClockInOutDto>> GetTodayByEmployeeAsync(string employeeCode, int companyId, int regionId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var data = await _unitOfWork.Repository<ClockInOut>().GetAllAsync();

            return data
                .Where(x => x.EmployeeCode == employeeCode &&
                            x.CompanyId == companyId &&
                            x.RegionId == regionId &&
                            x.AttendanceDate == today)
                .OrderBy(x => x.ActionTime)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<ClockInOutDto> AddAsync(ClockInOutCreateDto dto, int userId)
        {
            var entity = new ClockInOut
            {
                RegionId = dto.RegionId,
                CompanyId = dto.CompanyId,
                EmployeeCode = dto.EmployeeCode,
                EmployeeName = dto.EmployeeName,
                Department = dto.Department,
                AttendanceDate = DateOnly.FromDateTime(dto.AttendanceDate),
                ActionType = dto.ActionType,
                ActionTime = TimeSpan.Parse(dto.ActionTime),
                Status = dto.ActionType == "ClockIn" ? "Present" : "Completed",
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<ClockInOut>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var entity = await _unitOfWork.Repository<ClockInOut>().GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.Repository<ClockInOut>().Remove(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        private ClockInOutDto MapToDto(ClockInOut entity)
        {
            return new ClockInOutDto
            {
                ClockInOutId = entity.ClockInOutId,
                RegionId = entity.RegionId,
                CompanyId = entity.CompanyId,
                EmployeeCode = entity.EmployeeCode,
                EmployeeName = entity.EmployeeName,
                Department = entity.Department,
                AttendanceDate = entity.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                ActionType = entity.ActionType!,
                ActionTime = entity.ActionTime.ToString(@"hh\:mm"),
                Status = entity.Status
            };
        }
    }
}
