using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class EventService : IEventService
    {
        private readonly HRMSContext _context;

        public EventService(HRMSContext context)
        {
            _context = context;
        }

        // 🔹 Event Types
        public async Task<List<EventTypeDropdownDto>> GetEventTypesAsync()
        {
            return await _context.EventTypes
                .Where(x => x.IsActive)
                .OrderBy(x => x.EventTypeName)
                .Select(x => new EventTypeDropdownDto
                {
                    EventTypeId = x.EventTypeId,
                    EventTypeName = x.EventTypeName
                })
                .ToListAsync();
        }

        public async Task<List<EventListDto>> GetEventsAsync(int companyId, int regionId, int userId,int roleId)
        {
            var query =
                from e in _context.Events
                join r in _context.RoleMasters
                    on e.RoleId equals r.RoleId into roleJoin
                from r in roleJoin.DefaultIfEmpty()
                where e.IsActive
                      && (e.CompanyId == companyId || e.CompanyId == null)
                      && (e.RegionId == regionId || e.RegionId == null)
                      && (roleId == 0 || e.RoleId == roleId || e.RoleId == null)
                orderby e.EventDate descending
                select new EventListDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventTypeName = e.EventType.EventTypeName,
                    EventDate = e.EventDate,
                    Description = e.Description,
                    RoleId = e.RoleId ?? 0,
                    RoleName = r != null ? r.RoleName : "All"
                };

            return await query.ToListAsync();
        }




        // 🔹 Get By Id
        public async Task<EventDto?> GetEventByIdAsync(int eventId)
        {
            var entity = await _context.Events
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.IsActive);

            if (entity == null) return null;

            return new EventDto
            {
                EventId = entity.EventId,
                EventName = entity.EventName,
                EventTypeId = entity.EventTypeId,
                EventDate = entity.EventDate,
                Description = entity.Description
            };
        }

        // 🔹 CREATE
        public async Task<int> CreateEventAsync(EventDto dto)
        {
            var entity = new Event
            {
                EventName = dto.EventName,
                EventTypeId = dto.EventTypeId,
                EventDate = dto.EventDate,
                Description = dto.Description,
                RoleId = dto.RoleId == 0 ? null : dto.RoleId,
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                UserId = dto.UserID,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _context.Events.Add(entity);
            await _context.SaveChangesAsync();

            return entity.EventId;
        }

        // 🔹 UPDATE
        public async Task<bool> UpdateEventAsync(int eventId, EventDto dto)
        {
            var entity = await _context.Events
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.IsActive);

            if (entity == null) return false;

            entity.EventName = dto.EventName;
            entity.EventTypeId = dto.EventTypeId;
            entity.EventDate = dto.EventDate;
            entity.Description = dto.Description;
            entity.RoleId = dto.RoleId == 0 ? null : dto.RoleId;
            entity.CompanyId = dto.CompanyID;
            entity.RegionId = dto.RegionID;
            entity.UserId = dto.UserID;
            entity.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 DELETE (Soft Delete)
        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var entity = await _context.Events
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.IsActive);

            if (entity == null) return false;

            entity.IsActive = false;
            entity.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }


}
