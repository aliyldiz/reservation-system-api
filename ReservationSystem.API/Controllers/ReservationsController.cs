using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Application.DTOs;
using ReservationSystem.Application.Services;

namespace ReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationService _reservationService;

        public ReservationsController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationResponseDto>>> GetUserReservations()
        {
            var reservations = await _reservationService.GetReservationsForUserAsync(string.Empty);
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationResponseDto>> GetReservation(int id)
        {
            var reservationDto = await _reservationService.GetReservationByIdAsync(id);
            if (reservationDto == null)
            {
                return NotFound();
            }
            return Ok(reservationDto);
        }

        [HttpPost]
        public async Task<ActionResult<ReservationResponseDto>> CreateReservation([FromBody] CreateReservationDto createReservationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reservationDto = await _reservationService.CreateReservationAsync(createReservationDto, string.Empty);
            if (reservationDto == null)
            {
                return BadRequest(new { Message = "Event not found or not enough tickets available." });
            }
            return CreatedAtAction(nameof(GetReservation), new { id = reservationDto.Id }, reservationDto);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var result = await _reservationService.CancelReservationAsync(id, string.Empty);
            if (!result)
            {
                return BadRequest(new { Message = "Reservation not found or already cancelled." });
            }
            return NoContent();
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmReservation(int id)
        {
            var result = await _reservationService.ConfirmReservationAsync(id);
            if (!result)
            {
                return BadRequest(new { Message = "Reservation not found or not in pending state or hold expired." });
            }
            return Ok(new { Message = "successful" });
        }
    }
}
