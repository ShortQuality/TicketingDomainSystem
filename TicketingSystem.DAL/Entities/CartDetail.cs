
namespace TicketingSystem.DAL.Entities
{
    public class CartDetail
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int EventId { get; set; }
        public int SeatId { get; set; }
        public int PriceId { get; set; }
        public Cart Cart { get; set; }
    }
}
