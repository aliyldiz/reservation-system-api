using ReservationSystem.Application.DTOs;
using ReservationSystem.Application.Interfaces;
using ReservationSystem.Domain.Entities;
using ReservationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ReservationSystem.Application.Services
{
    public class ReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationService(IReservationRepository reservationRepository, IEventRepository eventRepository, UserManager<ApplicationUser> userManager)
        {
            _reservationRepository = reservationRepository;
            _eventRepository = eventRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<ReservationResponseDto>> GetReservationsForUserAsync(string userId)
        {
            var reservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);
            return reservations.Select(r => new ReservationResponseDto
            {
                Id = r.Id,
                EventId = r.EventId,
                EventName = r.Event?.Name,
                UserId = r.UserId,
                NumberOfTickets = r.NumberOfTickets,
                Status = r.Status,
                ReservationDate = r.ReservationDate
            });
        }

        public async Task<ReservationResponseDto?> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null) return null;

            return new ReservationResponseDto
            {
                Id = reservation.Id,
                EventId = reservation.EventId,
                EventName = reservation.Event?.Name,
                UserId = reservation.UserId,
                UserName = null,
                NumberOfTickets = reservation.NumberOfTickets,
                Status = reservation.Status,
                ReservationDate = reservation.ReservationDate
            };
        }

        public async Task<ReservationResponseDto?> CreateReservationAsync(CreateReservationDto createReservationDto, string userId)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(createReservationDto.EventId);
            if (eventEntity == null || eventEntity.AvailableTickets < createReservationDto.NumberOfTickets)
            {
                return null;
            }

            var reservation = new Reservation
            {
                EventId = createReservationDto.EventId,
                UserId = userId,
                NumberOfTickets = createReservationDto.NumberOfTickets,
                Status = ReservationStatus.Pending,
                ReservationDate = DateTime.UtcNow,
                HoldUntil = DateTime.UtcNow.AddMinutes(5)
            };

            await _reservationRepository.AddAsync(reservation);

            eventEntity.AvailableTickets -= createReservationDto.NumberOfTickets;
            await _eventRepository.UpdateAsync(eventEntity);

            return new ReservationResponseDto
            {
                Id = reservation.Id,
                EventId = reservation.EventId,
                EventName = eventEntity.Name,
                UserId = reservation.UserId,
                UserName = null,
                NumberOfTickets = reservation.NumberOfTickets,
                Status = reservation.Status,
                ReservationDate = reservation.ReservationDate
            };
        }

        public async Task<bool> CancelReservationAsync(int reservationId, string userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null || reservation.Status == ReservationStatus.Cancelled)
            {
                return false;
            }

            var eventEntity = await _eventRepository.GetByIdAsync(reservation.EventId);
            if (eventEntity == null) return false;

            reservation.Status = ReservationStatus.Cancelled;
            await _reservationRepository.UpdateAsync(reservation);

            eventEntity.AvailableTickets += reservation.NumberOfTickets;
            await _eventRepository.UpdateAsync(eventEntity);

            return true;
        }

        public async Task<bool> ConfirmReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null || reservation.Status != ReservationStatus.Pending || reservation.HoldUntil <= DateTime.UtcNow)
            {
                return false;
            }

            reservation.Status = ReservationStatus.Confirmed;
            await _reservationRepository.UpdateAsync(reservation);
            return true;
        }
    }
}
