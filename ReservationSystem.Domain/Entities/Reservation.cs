using ReservationSystem.Domain.Enums;

namespace ReservationSystem.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public string? UserId { get; set; }
        public int NumberOfTickets { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime? HoldUntil { get; set; }
    }
}
