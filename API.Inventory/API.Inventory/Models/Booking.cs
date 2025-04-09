using System;
using System.Collections.Generic;

namespace API.Inventory.Models
{
    public partial class Booking
    {
        public int Id { get; set; }
        public int? Memberid { get; set; }
        public int? Inventoryid { get; set; }
        public string Bookingreference { get; set; } = null!;
        public DateTime? Bookingdate { get; set; }
        public string? Status { get; set; }

        public virtual Inventory? Inventory { get; set; }
        public virtual Member? Member { get; set; }
    }
}
