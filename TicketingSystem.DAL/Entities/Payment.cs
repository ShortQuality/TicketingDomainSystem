
namespace TicketingSystem.DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
