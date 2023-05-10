
namespace TicketingSystem.DAL.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Venue Venue { get; set; }
    }
}
