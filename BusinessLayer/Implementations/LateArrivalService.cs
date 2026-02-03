using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;

public class LateArrivalService : ILateArrivalService
{
    private readonly HRMSContext _context;

    public LateArrivalService(HRMSContext context)
    {
        _context = context;
    }
   

    public async Task<List<LateArrivalDto>> GetLateArrivalsAsync(
 int companyId,
 int regionId,
 DateOnly fromDate,
 DateOnly toDate,
 string? employeeCode)
    {
        var clockRepo = _context.ClockInOuts;
        var shiftAllocRepo = _context.ShiftAllocations;
        var shiftRepo = _context.ShiftMasters;

        var clockData = await clockRepo
            .Where(c => c.ClockInTime != null)
            .GroupBy(c => new
            {
                c.EmployeeCode,
                c.AttendanceDate
            })
            .Select(g => g
                .OrderBy(x => x.ClockInTime)
                .First())
            .ToListAsync();
        var allocations = await shiftAllocRepo.ToListAsync();
        var shifts = await shiftRepo.ToListAsync();

        // STEP 1: Join & filter (DB-safe only)
        var joinedData = (
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
            }
        ).ToList(); // 🔥 THIS IS THE KEY LINE


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
 


}
