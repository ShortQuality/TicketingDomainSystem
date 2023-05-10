using System;

namespace TicketingSystem.DAL.Entities
{
    public class Seat
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Price { get; set; }
        public SeatRow SeatRow { get; set; }
    }
}
