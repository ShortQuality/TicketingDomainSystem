using Microsoft.AspNetCore.Mvc;
using System.Xml;
using TicketingSystem.BL;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;

namespace TicketingDomainSystem.Controllers
{
    [ApiController]
    [Route("[payments]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("payments/{paymentId}")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetPayment(int paymentId)
        {
            var payment = (await _unitOfWork.PaymentsRepository.GetAsync(
                filter: s => s.Id == paymentId)).FirstOrDefault();
            return Ok(payment.Status);
        }

        [HttpPost("{paymentId}/complete")]
        public async Task<ActionResult> CompletePayment(int paymentId)
        {
            var payment = (await _unitOfWork.PaymentsRepository
                .GetAsync(p => p.Id == paymentId, includeProperties: p => p.Cart.Tickets)).FirstOrDefault();

            if (payment == null)
            {
                return NotFound();
            }
            payment.Status = (int)PaymentStatus.Completed;
            foreach (var ticket in payment.Cart.Tickets)
            {
                ticket.Seat.SeatState = (int)SeatState.Sold;
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpPost("{paymentId}/failed")]
        public async Task<ActionResult> FailPayment(int paymentId)
        {
            var payment = (await _unitOfWork.PaymentsRepository
                .GetAsync(p => p.Id == paymentId, includeProperties: p => p.Cart.Tickets)).FirstOrDefault();

            if (payment == null)
            {
                return NotFound();
            }
            payment.Status = (int)PaymentStatus.Failed;
            foreach (var ticket in payment.Cart.Tickets)
            {
                ticket.Seat.SeatState = (int)SeatState.Available;
            }
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
