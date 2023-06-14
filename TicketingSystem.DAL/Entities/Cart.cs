
namespace TicketingSystem.DAL.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public float TotalAmount { get; set; }
        public User User { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public int Version { get; set; }
    }
}
