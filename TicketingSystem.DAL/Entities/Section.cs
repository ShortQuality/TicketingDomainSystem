using System;

namespace TicketingSystem.DAL.Entities
{
    public class Section
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public char Letter { get; set; }
        public int VenueId { get; set; }
        public Venue Venue { get; set; }
        public int Version { get; set; }
        public ICollection<SeatRow> SeatRows { get; set; }
    }
}
