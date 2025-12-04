using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Application.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task AddAsync(Reservation entity);
        Task UpdateAsync(Reservation entity);
        Task DeleteAsync(Reservation entity);
        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId);
        Task<IEnumerable<Reservation>> GetReservationsByEventIdAsync(int eventId);
    }
}
