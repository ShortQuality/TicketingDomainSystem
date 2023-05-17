using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketingSystem.DAL.Entities;

namespace TicketingSystem.BL.Services.Interfaces
{
    public interface ICartService
    {
        Cart DeleteTicketsFromCart(Cart cart, Ticket ticketForRemoving);
        Cart BookSeatsFromCart(Cart cart);
    }
}
