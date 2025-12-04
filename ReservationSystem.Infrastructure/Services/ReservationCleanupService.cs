using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReservationSystem.Application.Interfaces;
using ReservationSystem.Domain.Enums;
using ReservationSystem.Persistence.Context;

namespace ReservationSystem.Infrastructure.Services
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationCleanupService> _logger;

        public ReservationCleanupService(IServiceProvider serviceProvider, ILogger<ReservationCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Cleanup Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Reservation Cleanup Service doing work.");

                try
                {
                    await DoWork(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing Reservation Cleanup Service.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Reservation Cleanup Service stopped.");
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();

                var expiredPendingReservations = await context.Reservations
                    .Include(r => r.Event)
                    .Where(r => r.Status == ReservationStatus.Pending && r.HoldUntil.HasValue && r.HoldUntil.Value <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                foreach (var reservation in expiredPendingReservations)
                {
                    _logger.LogInformation($"Cancelling expired pending reservation ID: {reservation.Id} for Event ID: {reservation.EventId}");

                    reservation.Status = ReservationStatus.Cancelled;
                    await reservationRepository.UpdateAsync(reservation);

                    if (reservation.Event != null)
                    {
                        reservation.Event.AvailableTickets += reservation.NumberOfTickets;
                        await eventRepository.UpdateAsync(reservation.Event);
                    }
                }
            }
        }
    }
}
