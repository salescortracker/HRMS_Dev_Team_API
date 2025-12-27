
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Newtonsoft.Json;

namespace BusinessLayer.Implementations
{
    public class TimesheetService : ITimesheetService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TimesheetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<LoggedInUserDto> GetLoggedInUserAsync(int userId)
        {
            var user = (await _unitOfWork.Repository<User>()
                .GetAllAsync())
                .FirstOrDefault(x => x.UserId == userId);

            if (user == null)
                return new LoggedInUserDto();

            return new LoggedInUserDto
            {
                UserId = user.UserId,
                EmployeeName = user.FullName,
                EmployeeCode = user.EmployeeCode   // or EmpCode column
            };
        }
        public async Task<int> SaveTimesheetAsync(TimesheetRequestDto dto)
        {
            // 🔹 get manager from Users table
            var user = await _unitOfWork.Repository<User>()
                .GetByIdAsync(dto.UserId);

            var timesheet = new Timesheet
            {
                UserId = dto.UserId,
                ManagerUserId = user?.ReportingTo,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                EmployeeCode = dto.EmployeeCode,
                EmployeeName = dto.EmployeeName,
                TimesheetDate = DateOnly.FromDateTime(dto.TimesheetDate),
                Comments = dto.Comments,
                FileName = dto.FileName ?? "",
                FilePath = dto.FilePath,
                Status = "Pending",
                CreatedBy = dto.UserId,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<Timesheet>().AddAsync(timesheet);
            await _unitOfWork.CompleteAsync();

            // 🔹 Save projects
            foreach (var p in dto.Projects)
            {
                var project = new TimesheetProject
                {
                    TimesheetId = timesheet.TimesheetId,
                    ProjectName = p.ProjectName,
                    StartTime = TimeOnly.Parse(p.StartTime),
                    EndTime = TimeOnly.Parse(p.EndTime),
                    TotalMinutes = p.TotalMinutes,
                    TotalHoursText = p.TotalHoursText,
                    Otminutes = p.OTMinutes,
                    OthoursText = p.OTHoursText,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Repository<TimesheetProject>().AddAsync(project);
            }

            await _unitOfWork.CompleteAsync();
            return timesheet.TimesheetId;
        }

        // ✅ GET MY TIMESHEETS (USER-WISE)
        public async Task<IEnumerable<TimesheetListDto>> GetMyTimesheetsAsync(int userId)
        {
            var timesheets = await _unitOfWork.Repository<Timesheet>()
                .FindAsync(x => x.UserId == userId);

            var timesheetIds = timesheets.Select(t => t.TimesheetId).ToList();

            var projects = await _unitOfWork.Repository<TimesheetProject>()
                .FindAsync(p => timesheetIds.Contains(p.TimesheetId));

            return timesheets.Select(t => new TimesheetListDto
            {
                TimesheetId = t.TimesheetId,
                EmployeeName = t.EmployeeName,
                EmployeeCode = t.EmployeeCode,
                TimesheetDate = t.TimesheetDate.ToDateTime(TimeOnly.MinValue),
                Status = t.Status,

                // 🔥 MANUAL JOIN
                Projects = projects
                    .Where(p => p.TimesheetId == t.TimesheetId)
                    .Select(p => new TimesheetProjectDto
                    {
                        ProjectName = p.ProjectName,
                        StartTime = p.StartTime.ToString(),
                        EndTime = p.EndTime.ToString(),
                        TotalMinutes = p.TotalMinutes,
                        TotalHoursText = p.TotalHoursText,
                        OTMinutes = p.Otminutes,
                        OTHoursText = p.OthoursText
                    })
                    .ToList()
            });
        }


    }
}
