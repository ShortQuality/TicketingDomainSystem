using System;

namespace TicketingSystem.DAL.Entities
{
    public class Seat
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public bool isBooked { get; set; }
        public int SeatRowId { get; set; }
        public SeatRow SeatRow { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
