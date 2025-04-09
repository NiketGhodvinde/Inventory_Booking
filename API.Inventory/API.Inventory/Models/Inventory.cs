using System;
using System.Collections.Generic;

namespace API.Inventory.Models
{
    public partial class Inventory
    {
        public Inventory()
        {
            Bookings = new HashSet<Booking>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Remainingcount { get; set; }
        public DateOnly Expirationdate { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
