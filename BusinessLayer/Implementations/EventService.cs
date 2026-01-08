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
    public class EventService : IEventService
    {
        public readonly HRMSContext _context;
        public EventService(HRMSContext context)
        {
            _context = context;
        }
        public async Task<List<EventTypeDto>> GetEventTypesAsync(int companyId, int regionId)
        {
            return await _context.EventTypes
                .Where(x => x.CompanyId == companyId &&
                            x.RegionId == regionId &&
                            x.IsActive)
                .Select(x => new EventTypeDto
                {
                    EventTypeId = x.EventTypeId,
                    EventTypeName = x.EventTypeName
                })
                .ToListAsync();
        }

        public async Task<List<EventTypeDto>> GetAllEventTypesAsync()
        {
            return await _context.EventTypes
                .Select(x => new EventTypeDto
                {
                    EventTypeId = x.EventTypeId,
                    EventTypeName = x.EventTypeName
                })
                .ToListAsync();
        }

        // ✅ EVENTS LIST
        public async Task<List<Event>> GetEventsAsync(int companyId, int regionId)
        {
            return await _context.Events
                .Where(x => x.CompanyId == companyId &&
                            x.RegionId == regionId &&
                            x.IsActive)
                .OrderByDescending(x => x.EventDate)
                .ToListAsync();
        }
        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
            return await _context.Events
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.IsActive);
        }

        // ✅ CREATE EVENT
        public async Task<Event> CreateEventAsync(EventDto dto, int userId)
        {
            var entity = new Event
            {
                EventName = dto.EventName,
                EventTypeId = dto.EventTypeId,
                EventDate = DateOnly.FromDateTime(dto.EventDate),
                Description = dto.Description,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                UserId= userId,
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _context.Events.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
        public async Task<bool> UpdateEventAsync(int eventId, EventDto dto, int userId)
        {
            var eventEntity = await _context.Events
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.IsActive);

            if (eventEntity == null)
                return false;

            eventEntity.EventName = dto.EventName;
            eventEntity.EventTypeId = dto.EventTypeId;
            eventEntity.EventDate = DateOnly.FromDateTime(dto.EventDate);
            eventEntity.Description = dto.Description;
            eventEntity.ModifiedBy = userId;
            eventEntity.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteEventAsync(int eventId, int userId)
        {
            var eventEntity = await _context.Events
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.IsActive);

            if (eventEntity == null)
                return false;

            eventEntity.IsActive = false;
            eventEntity.ModifiedBy = userId;
            eventEntity.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}

