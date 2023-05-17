using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;

namespace TicketingDomainSystem.Controllers
{
    [ApiController]
    [Route("[venues]")]
    public class VenuesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public VenuesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venue>>> GetVenues()
        {
            var venues = await _unitOfWork.VenuesRepository.GetAsync();
            return Ok(venues.AsEnumerable());
        }

        [HttpGet("{venueId}/sections")]
        public async Task<ActionResult<IEnumerable<Section>>> GetSections(int venueId)
        {
            var sections = await _unitOfWork.SectionsRepository.GetAsync(x => x.VenueId == venueId);
            return Ok(sections);
        }
    }
}
