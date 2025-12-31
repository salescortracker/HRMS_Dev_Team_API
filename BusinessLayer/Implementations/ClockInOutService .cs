using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Models;
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
            var clockRepo = _unitOfWork.Repository<ClockInOut>();
            var deptRepo = _unitOfWork.Repository<RoleMaster>(); // Assuming you have a Department entity

            var clockData = await clockRepo.GetAllAsync();
            var deptData = await deptRepo.GetAllAsync();

            var result = (from c in clockData
                          join d in deptData
                          on c.Department equals d.RoleId
                          where 
                                
                                c.AttendanceDate == today
                          orderby c.ActionTime
                          select new ClockInOutDto
                          {
                              ClockInOutId = c.ClockInOutId,
                              RegionId = c.RegionId,
                              CompanyId = c.CompanyId,
                              EmployeeCode = c.EmployeeCode,
                              EmployeeName = c.EmployeeName,
                              Departments = d.RoleName, 
                              AttendanceDate = c.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                              ActionType = c.ActionType,
                              ActionTime = c.ActionTime.ToString(@"hh\:mm"),
                              Status = c.Status
                          }).ToList();

            return result;
        }


        public async Task<ClockInOutDto> AddAsync(ClockInOutCreateDto dto, int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var actionTime = TimeOnly.Parse(dto.ActionTime);

            var repo = _unitOfWork.Repository<ClockInOut>();

            var entity = new ClockInOut
            {
                RegionId = dto.RegionId,
                CompanyId = dto.CompanyId,
                EmployeeCode = dto.EmployeeCode,
                EmployeeName = dto.EmployeeName,
                Department = dto.Department,

                AttendanceDate = today,
                ActionType = dto.ActionType,
                ActionTime = actionTime,

                ClockInTime = dto.ActionType == "ClockIn" ? actionTime : null,
                ClockOutTime = dto.ActionType == "ClockOut" ? actionTime : null,

                TotalMinutes = 0,
                Status = dto.ActionType == "ClockIn" ? "Present" : "Completed",

                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            await repo.AddAsync(entity);
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
            var duration = TimeSpan.FromMinutes(entity.TotalMinutes);

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

                ClockInTime = entity.ClockInTime?.ToString(@"hh\:mm"),
                ClockOutTime = entity.ClockOutTime?.ToString(@"hh\:mm"),

                TotalMinutes = entity.TotalMinutes,
                TotalDuration = $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}",

                Status = entity.Status
            };
        }

        public async Task<List<EarlyDepartureDto>> GetEarlyDeparturesAsync(
          int companyId,
          int regionId,
          DateOnly fromDate,
          DateOnly toDate,
          string? employeeCode)
        {
            var clockInOutRepo = _unitOfWork.Repository<ClockInOut>();
            var shiftAllocationRepo = _unitOfWork.Repository<ShiftAllocation>();
            var shiftRepo = _unitOfWork.Repository<ShiftMaster>();

            var clockRecords = await clockInOutRepo.GetAllAsync();
            var allocations = await shiftAllocationRepo.GetAllAsync();
            var shifts = await shiftRepo.GetAllAsync();

            var query =
                from c in clockRecords
                join sa in allocations on c.EmployeeCode equals sa.EmployeeCode
                join s in shifts on sa.ShiftId equals s.ShiftId
                where c.CompanyId == companyId
                      && c.RegionId == regionId
                      && c.AttendanceDate >= fromDate
                      && c.AttendanceDate <= toDate
                      && c.ClockOutTime != null
                      && sa.IsActive
                      && sa.StartDate <= c.AttendanceDate
                      && (sa.EndDate == null || sa.EndDate >= c.AttendanceDate)
                      && c.ClockOutTime < s.ShiftEndTime
                select new { c, s };

            // Optional employee filter
            if (!string.IsNullOrWhiteSpace(employeeCode))
            {
                query = query.Where(x => x.c.EmployeeCode == employeeCode);
            }

            return query
                .Select(x => new EarlyDepartureDto
                {
                    EmployeeCode = x.c.EmployeeCode,
                    EmployeeName = x.c.EmployeeName,
                    Department = x.c.Department,

                    AttendanceDate = x.c.AttendanceDate,

                    ShiftName = x.s.ShiftName,
                    ShiftEndTime = x.s.ShiftEndTime,
                    ClockOutTime = x.c.ClockOutTime!.Value,

                    EarlyByMinutes =
                        (int)(x.s.ShiftEndTime - x.c.ClockOutTime!.Value).TotalMinutes
                })
                .OrderBy(x => x.AttendanceDate)
                .ThenBy(x => x.EmployeeName)
                .ToList();
        }
        public async Task<List<LateArrivalDto>> GetLateArrivalsAsync(
       int companyId,
       int regionId,
       DateOnly fromDate,
       DateOnly toDate,
       string? employeeCode)
        {
            var clockRepo = _unitOfWork.Repository<ClockInOut>();
            var shiftAllocRepo = _unitOfWork.Repository<ShiftAllocation>();
            var shiftRepo = _unitOfWork.Repository<ShiftMaster>();

            var clockData = await clockRepo.GetAllAsync();
            var allocations = await shiftAllocRepo.GetAllAsync();
            var shifts = await shiftRepo.GetAllAsync();

            // STEP 1: Join & filter (NO time logic here)
            var joinedData =
                from c in clockData
                join sa in allocations on c.EmployeeCode equals sa.EmployeeCode
                join sm in shifts on sa.ShiftId equals sm.ShiftId
                where c.CompanyId == companyId
                      && c.RegionId == regionId
                      && c.AttendanceDate >= fromDate
                      && c.AttendanceDate <= toDate
                      && sa.IsActive
                      && sm.IsActive
                      && c.ClockInTime != null
                      && (employeeCode == null || c.EmployeeCode == employeeCode)
                      && sa.StartDate <= c.AttendanceDate
                      && (sa.EndDate == null || sa.EndDate >= c.AttendanceDate)
                select new
                {
                    c.EmployeeName,
                    c.EmployeeCode,
                    c.AttendanceDate,
                    c.ClockInTime,
                    sm.ShiftStartTime,
                    sm.GraceTime
                };

            // STEP 2: Apply time calculations IN MEMORY
            var result = joinedData
                .Where(x =>
                {
                    var allowedTime =
                        x.ShiftStartTime.AddMinutes(x.GraceTime ?? 0);

                    return x.ClockInTime > allowedTime;
                })
                .Select(x =>
                {
                    var allowedTime =
                        x.ShiftStartTime.AddMinutes(x.GraceTime ?? 0);

                    return new LateArrivalDto
                    {
                        EmployeeName = x.EmployeeName,
                        EmployeeCode = x.EmployeeCode,
                        AttendanceDate = x.AttendanceDate,
                        ShiftStartTime = x.ShiftStartTime,
                        AllowedClockInTime = allowedTime,
                        ClockInTime = x.ClockInTime,
                        LateByMinutes = (int)(
                            x.ClockInTime!.Value.ToTimeSpan()
                            - allowedTime.ToTimeSpan()
                        ).TotalMinutes,
                        Status = "Late"
                    };
                })
                .OrderBy(x => x.AttendanceDate)
                .ThenBy(x => x.EmployeeName)
                .ToList();

            return result;
        }

        public async Task<List<EmployeeDropdownDto>> GetEmployeesByCompanyRegionAsync(
            int companyId,
            int regionId)
        {
            var usersRepo = _unitOfWork.Repository<DataAccessLayer.DBContext.User>();

            var users = await usersRepo.GetAllAsync();

            return users
                .Where(u =>
                    u.CompanyId == companyId &&
                    u.RegionId == regionId &&
                    u.Status == "Active")
                .OrderBy(u => u.FullName)
                .Select(u => new EmployeeDropdownDto
                {
                    UserId = u.UserId,
                    EmployeeCode = u.EmployeeCode,
                    FullName = u.FullName
                })
                .ToList();
        }


    }
}
