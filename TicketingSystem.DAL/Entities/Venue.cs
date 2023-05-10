
namespace TicketingSystem.DAL.Entities
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Section> Sections { get; set; }
    }
}
