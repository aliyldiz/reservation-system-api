using Microsoft.EntityFrameworkCore;
using ReservationSystem.Application.Interfaces;
using ReservationSystem.Domain.Entities;
using ReservationSystem.Persistence.Context;

namespace ReservationSystem.Persistence.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task AddAsync(Reservation entity)
        {
            await _context.Reservations.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation entity)
        {
            _context.Reservations.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Reservation entity)
        {
            _context.Reservations.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(string userId)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Event)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByEventIdAsync(int eventId)
        {
            return await _context.Reservations
                .Where(r => r.EventId == eventId)
                .Include(r => r.Event)
                .ToListAsync();
        }
    }
}
