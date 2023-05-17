
namespace TicketingSystem.DAL.Entities
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Event Event { get; set; }
        public ICollection<Section> Sections { get; set; }
    }
}
