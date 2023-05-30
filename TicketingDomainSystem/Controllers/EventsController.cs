using IdentityServer3.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;

namespace TicketingDomainSystem.Controllers
{
    [ApiController]
    [Route("[events]")]
    public class EventsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public EventsController(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        //{
        //    var events = await _unitOfWork.EventsRepository.GetAsync();
        //    return Ok(events);
        //}

        //[HttpGet("{eventId}/sections/{sectionId}/seats")]
        //public async Task<ActionResult<IEnumerable<Seat>>> GetSeats(int eventId, int sectionId)
        //{
        //    var seats = await _unitOfWork.SeatsRepository.GetAsync(
        //        filter: s => s.Id == sectionId && s.SeatRow.Section.Venue.Event.Id == eventId);
        //    return Ok(seats);
        //}

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var cacheKey = "GetEvents";
            var cachedEvents = _cache.Get<IEnumerable<Event>>(cacheKey);

            if (cachedEvents != null)
            {
                return Ok(cachedEvents);
            }

            var events = await _unitOfWork.EventsRepository.GetAsync();

            _cache.Set(cacheKey, events, TimeSpan.FromSeconds(60)); // Cache the response for 60 seconds

            return Ok(events);
        }

        [HttpGet("{eventId}/sections/{sectionId}/seats")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeats(int eventId, int sectionId)
        {
            var cacheKey = $"seats_{eventId}_{sectionId}";
            var cachedSeats = _cache.Get<IEnumerable<Seat>>(cacheKey);

            if (cachedSeats != null)
            {
                return Ok(cachedSeats);
            }

            var seats = await _unitOfWork.SeatsRepository.GetAsync(
                filter: s => s.Id == sectionId && s.SeatRow.Section.Venue.Event.Id == eventId);

            _cache.Set(cacheKey, seats, TimeSpan.FromSeconds(60));

            return Ok(seats);
        }
    }
}
