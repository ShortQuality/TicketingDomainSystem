using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketingSystem.BL.Services.Interfaces;
using TicketingSystem.DAL.Entities;

namespace TicketingSystem.BL.Services
{
    public class CartService : ICartService
    {
        public Cart DeleteTicketsFromCart(Cart cart, Ticket ticketForRemoving)
        {
            cart.Tickets.Remove(ticketForRemoving);
            return cart;
        }

        public Cart BookSeatsFromCart(Cart cart)
        {
            foreach(var ticket in cart.Tickets)
            {
                ticket.Seat.SeatState = (int)SeatState.Booked;
            }

            return cart;
        }
    }
}
