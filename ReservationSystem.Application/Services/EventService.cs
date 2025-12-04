using ReservationSystem.Application.DTOs;
using ReservationSystem.Application.Interfaces;
using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Application.Services
{
    public class EventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return events.Select(e => new EventResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Date = e.Date,
                Location = e.Location,
                AvailableTickets = e.AvailableTickets
            });
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(int id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null) return null;

            return new EventResponseDto
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                Date = eventEntity.Date,
                Location = eventEntity.Location,
                AvailableTickets = eventEntity.AvailableTickets
            };
        }

        public async Task<EventResponseDto> CreateEventAsync(CreateEventDto createEventDto)
        {
            var eventEntity = new Event
            {
                Name = createEventDto.Name,
                Description = createEventDto.Description,
                Date = createEventDto.Date,
                Location = createEventDto.Location,
                AvailableTickets = createEventDto.AvailableTickets
            };

            await _eventRepository.AddAsync(eventEntity);

            return new EventResponseDto
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                Date = eventEntity.Date,
                Location = eventEntity.Location,
                AvailableTickets = eventEntity.AvailableTickets
            };
        }

        public async Task<bool> UpdateEventAsync(int id, CreateEventDto updateEventDto)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null) return false;

            eventEntity.Name = updateEventDto.Name;
            eventEntity.Description = updateEventDto.Description;
            eventEntity.Date = updateEventDto.Date;
            eventEntity.Location = updateEventDto.Location;
            eventEntity.AvailableTickets = updateEventDto.AvailableTickets;

            await _eventRepository.UpdateAsync(eventEntity);
            return true;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null) return false;

            await _eventRepository.DeleteAsync(eventEntity);
            return true;
        }
    }
}
