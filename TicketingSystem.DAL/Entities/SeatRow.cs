
namespace TicketingSystem.DAL.Entities
{
    public class SeatRow
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
