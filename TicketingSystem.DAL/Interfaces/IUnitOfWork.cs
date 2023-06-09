﻿using TicketingSystem.DAL.Entities;

namespace TicketingSystem.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Cart> CartsRepository { get; }
        IRepository<CartDetail> CartDetailsRepository { get; }
        IRepository<Event> EventsRepository { get; }
        IRepository<Payment> PaymentsRepository{ get; }
        IRepository<Price> PricesRepository{ get; }
        IRepository<Seat> SeatsRepository { get; }
        IRepository<SeatRow> SeatRowsRepository{ get; }
        IRepository<Section> SectionsRepository{ get; }
        IRepository<Venue> VenuesRepository{ get; }
        Task SaveAsync();
    }
}
