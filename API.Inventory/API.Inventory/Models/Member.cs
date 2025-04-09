using System;
using System.Collections.Generic;

namespace API.Inventory.Models
{
    public partial class Member
    {
        public Member()
        {
            Bookings = new HashSet<Booking>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int? Bookingcount { get; set; }
        public DateTime Datejoined { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
