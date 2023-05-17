
namespace TicketingSystem.DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public int CartId { get; set; }
        public int UserId { get; set; }
        public Cart Cart { get; set; }
        public User User { get; set; }
    }
}
