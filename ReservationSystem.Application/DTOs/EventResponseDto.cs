namespace ReservationSystem.Application.DTOs
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int AvailableTickets { get; set; }
    }
}
