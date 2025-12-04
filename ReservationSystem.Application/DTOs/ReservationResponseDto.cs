using ReservationSystem.Domain.Enums;

namespace ReservationSystem.Application.DTOs
{
    public class ReservationResponseDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int NumberOfTickets { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}
