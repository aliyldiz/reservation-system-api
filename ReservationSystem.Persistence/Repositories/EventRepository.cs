using Microsoft.EntityFrameworkCore;
using ReservationSystem.Application.Interfaces;
using ReservationSystem.Domain.Entities;
using ReservationSystem.Persistence.Context;

namespace ReservationSystem.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task AddAsync(Event entity)
        {
            await _context.Events.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event entity)
        {
            _context.Events.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Event entity)
        {
            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Event?> GetEventWithReservationsAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Reservations)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
