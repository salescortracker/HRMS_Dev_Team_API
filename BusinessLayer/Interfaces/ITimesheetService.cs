
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface ITimesheetService
    {
        Task<LoggedInUserDto> GetLoggedInUserAsync(int userId);
        Task<int> SaveTimesheetAsync(TimesheetRequestDto dto);
        Task<IEnumerable<TimesheetListDto>> GetMyTimesheetsAsync(int userId);
    }
}
