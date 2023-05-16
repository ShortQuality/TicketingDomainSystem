using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketingSystem.DAL.Entities
{
    public class User
    {
        public bool IsAdmin { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Cart Cart { get; set; }
        public Payment Payment { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
