using System.ComponentModel.DataAnnotations;

namespace ReservationSystem.Application.DTOs
{
    public class CreateReservationDto
    {
        public int EventId { get; set; }
        public int NumberOfTickets { get; set; }
    }
}
