using IdentityServer3.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;

        public OrdersController(IUnitOfWork unitOfWork, ICartService cartService, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _cache = cache;
        }

        [HttpGet("carts/{cartId}")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartDetails(int cartId)
        {
            var cacheKey = $"cart_{cartId}";
            var cachedCartItems = _cache.Get<IEnumerable<Cart>>(cacheKey);

            if (cachedCartItems != null)
            {
                return Ok(cachedCartItems);
            }

            var cartItems = await _unitOfWork.CartsRepository.GetAsync(
                filter: cart => cart.Id == cartId,
                includeProperties: cart => cart.Tickets);

            _cache.Set(cacheKey, cartItems, TimeSpan.FromSeconds(60)); // Cache the response for 60 seconds

            // Invalidate the cache for the Event resource
            _cache.Remove("GetEvents");

            return Ok(cartItems);
        }

        [HttpPost("carts/{cartId}")]
        public async Task<ActionResult<Cart>> AddCartItem_PessimisticConcurrency(int cartId, Cart cart)
        {
            var payment = new Payment
            {
                CartId = cartId,
                Cart = cart
            };

            _unitOfWork.PaymentsRepository.AddAsync(payment);

            cart.Version++;
            var lockAcquired = await _unitOfWork.CartsRepository.LockEntityAsync(cartId);

            if (!lockAcquired)
            {
                throw new ArgumentException($"Cart with ID {cartId} is currently being modified by another user.");
            }

            _unitOfWork.CartsRepository.AddAsync(cart);

            await _unitOfWork.SaveAsync();

            await _unitOfWork.CartsRepository.UnlockEntityAsync(cartId);
            // Invalidate the cache for the Event resource
            _cache.Remove("GetEvents");

            return CreatedAtAction(nameof(GetCartDetails), new { cartId = cart.Id }, cart);

        }

        [HttpPost("carts/{cartId}")]
        public async Task<ActionResult<Cart>> AddCartItem_OptimisticConcurrency(int cartId, Cart cartDto)
        {
            var payment = new Payment
            {
                CartId = cartId,
                Cart = cartDto
            };

            var cart = await _unitOfWork.CartsRepository.GetAsync(cart => cart.Id == cartId);

            if (cart == null)
            {
                return NotFound();
            }

            // Check if the cart has been modified since it was retrieved
            if (cart.FirstOrDefault().Version != cartDto.Version)
            {
                return Conflict();
            }

            _unitOfWork.PaymentsRepository.AddAsync(payment);
            _unitOfWork.CartsRepository.AddAsync(cartDto);

            await _unitOfWork.SaveAsync();

            // Invalidate the cache for the Event resource
            _cache.Remove("GetEvents");

            return CreatedAtAction(nameof(GetCartDetails), new { cartId = cart.Id }, cart);

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
