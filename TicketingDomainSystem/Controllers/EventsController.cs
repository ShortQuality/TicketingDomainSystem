using IdentityServer3.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TicketingSystem.DAL;
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
        private readonly TicketingSystemContext _dbContext;

        public EventsController(IUnitOfWork unitOfWork, IMemoryCache cache, TicketingSystemContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _dbContext = dbContext;
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

        [HttpPost("{eventId}/sections/{sectionId}")]
        public async Task<ActionResult<Cart>> AddVenueSectionToEvent_PessimisticConcurrency(int sectionId, Section sectionDto)
        {
            using var transaction = _dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
            try
            {
                _unitOfWork.SectionsRepository.AddAsync(sectionDto);
                await _unitOfWork.SaveAsync();
                transaction.Commit();
                return Ok();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(400);
            }
        }

        [HttpPost("carts/{cartId}")]
        public async Task<ActionResult<Cart>> AddVenueSectionToEvent_OptimisticConcurrency(int sectionId, Section sectionDto)
        {
            var section = await _unitOfWork.SectionsRepository.GetAsync(section => section.Id == sectionId);

            if (section == null)
            {
                return NotFound();
            }

            // Check if the cart has been modified since it was retrieved
            if (section.FirstOrDefault().Version != sectionDto.Version)
            {
                return Conflict();
            }

            sectionDto.Version++;
            _unitOfWork.SectionsRepository.AddAsync(sectionDto);

            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
