using Microsoft.AspNetCore.Mvc;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;

namespace TicketingDomainSystem.Controllers
{
    [ApiController]
    [Route("[events]")]
    public class EventsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _unitOfWork.EventsRepository.GetAsync();
            return Ok(events);
        }

        [HttpGet("{eventId}/sections/{sectionId}/seats")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeats(int eventId, int sectionId)
        {
            var seats = await _unitOfWork.SeatsRepository.GetAsync(
                filter: s => s.Id == sectionId && s.SeatRow.Section.Venue.Event.Id == eventId);
            return Ok(seats);
        }
    }
}
