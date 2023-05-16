
namespace TicketingSystem.DAL.Entities
{
    public class Price
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
