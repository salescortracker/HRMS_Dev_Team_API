using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;

namespace BusinessLayer.Interfaces
{
    public interface IEventService
    {
        Task<List<EventTypeDto>> GetEventTypesAsync(int companyId, int regionId);
        Task<List<EventTypeDto>> GetAllEventTypesAsync();

        Task<List<Event>> GetEventsAsync(int companyId, int regionId);
        Task<Event?> GetEventByIdAsync(int eventId);

        Task<Event> CreateEventAsync(EventDto dto, int userId);
        Task<bool> UpdateEventAsync(int eventId, EventDto dto, int userId);
        Task<bool> DeleteEventAsync(int eventId, int userId);
    }
}
