using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace HRMS_Backend.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("EventTypes")]
        public async Task<IActionResult> GetEventTypes()
        {
            return Ok(await _eventService.GetEventTypesAsync());
        }


        //[HttpGet]
        //public async Task<IActionResult> GetEvents()
        //{
        //    return Ok(await _eventService.GetEventsAsync());
        //}
        [HttpGet]
        public async Task<IActionResult> GetEvents(
                    int companyId,
                    int regionId,
                    int userId,
                    int roleId)
        {
            var data = await _eventService.GetEventsAsync(
                companyId, regionId, userId, roleId);

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var ev = await _eventService.GetEventByIdAsync(id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var eventId = await _eventService.CreateEventAsync(dto);
            return Ok(new { Message = "Event created successfully", EventId = eventId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _eventService.UpdateEventAsync(id, dto);
            if (!result) return NotFound();

            return Ok(new { Message = "Event updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventService.DeleteEventAsync(id);
            if (!result) return NotFound();

            return Ok(new { Message = "Event deleted successfully" });
        }
    }

}
