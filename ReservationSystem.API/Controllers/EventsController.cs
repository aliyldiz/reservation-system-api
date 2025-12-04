using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Application.DTOs;
using ReservationSystem.Application.Services;

namespace ReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventsController(EventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponseDto>> GetEvent(int id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null)
            {
                return NotFound();
            }
            return Ok(eventDto);
        }

        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventDto = await _eventService.CreateEventAsync(createEventDto);
            return CreatedAtAction(nameof(GetEvent), new { id = eventDto.Id }, eventDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventDto updateEventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _eventService.UpdateEventAsync(id, updateEventDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventService.DeleteEventAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
