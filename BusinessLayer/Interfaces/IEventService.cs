using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEventService
    {

        Task<List<EventTypeDropdownDto>> GetEventTypesAsync();

        //Task<List<EventListDto>> GetEventsAsync();

        Task<List<EventListDto>> GetEventsAsync(
      int companyId,
      int regionId,
      int userId,
      int roleId
  );
        Task<EventDto?> GetEventByIdAsync(int eventId);

        Task<int> CreateEventAsync(EventDto dto);
        Task<bool> UpdateEventAsync(int eventId, EventDto dto);
        Task<bool> DeleteEventAsync(int eventId);

    }
}
