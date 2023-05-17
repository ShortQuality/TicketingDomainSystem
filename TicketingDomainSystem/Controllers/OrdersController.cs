using Microsoft.AspNetCore.Mvc;
using TicketingSystem.BL.Services;
using TicketingSystem.BL.Services.Interfaces;
using TicketingSystem.DAL.Entities;
using TicketingSystem.DAL.Interfaces;

namespace TicketingDomainSystem.Controllers
{
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

        public OrdersController(IUnitOfWork unitOfWork, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
        }

        [HttpGet("carts/{cartId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartDetails(int cartId)
        {
            var cartItems = await _unitOfWork.CartsRepository.GetAsync(
                filter: cart => cart.Id == cartId,
                includeProperties: cart => cart.Tickets);
            return Ok(cartItems);
        }

        [HttpPost("carts/{cartId}")]
        public async Task<ActionResult<Cart>> AddCartItem(int cartId, Cart cart)
        {

            var payment = new Payment
            {
                CartId = cartId,
                Cart = cart
            };

            _unitOfWork.PaymentsRepository.AddAsync(payment);
            _unitOfWork.CartsRepository.AddAsync(cart);
            await _unitOfWork.SaveAsync();

            //var cartState = new
            //{
            //    TotalAmount = cart.TotalAmount
            //};

            //return Ok(cartState);
            return Ok();
        }

        [HttpDelete("carts/{cartId}/events/{eventId}/seats/{seatId}")]
        public async Task<IActionResult> DeleteCartItem(int cartId, int eventId, int seatId)
        {
            var cart = await _unitOfWork.CartsRepository.GetAsync(filter: cart => cart.Id == cartId);
            var ticketForRemoving = await _unitOfWork.TicketsRepository.GetAsync(
                filter: ticket => ticket.EventId == eventId && ticket.SeatId == seatId);
            
            var updatedCart = _cartService.DeleteTicketsFromCart(cart.FirstOrDefault(), ticketForRemoving.FirstOrDefault());

            _unitOfWork.CartsRepository.Update(updatedCart);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        [HttpPut("carts/{cartId}/book")]
        public async Task<ActionResult<Guid>> Book(int cartId)
        {
            var cart = await _unitOfWork.CartsRepository.GetAsync(filter: cart => cart.Id == cartId);


            _unitOfWork.CartsRepository.Update(_cartService.BookSeatsFromCart(cart.FirstOrDefault()));
            await _unitOfWork.SaveAsync();

            var paymentId = cart.FirstOrDefault().User.Payment.Id;

            return Ok(paymentId);
        }
    }
}
