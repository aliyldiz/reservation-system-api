using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(int id);
        Task<IEnumerable<Event>> GetAllAsync();
        Task AddAsync(Event entity);
        Task UpdateAsync(Event entity);
        Task DeleteAsync(Event entity);
        Task<Event?> GetEventWithReservationsAsync(int id);
    }
}
