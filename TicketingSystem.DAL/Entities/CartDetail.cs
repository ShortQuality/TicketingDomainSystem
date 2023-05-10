
namespace TicketingSystem.DAL.Entities
{
    public class CartDetail
    {
        public int CartId { get; set; }
        public int EventId { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public Cart Cart { get; set; }
    }
}
