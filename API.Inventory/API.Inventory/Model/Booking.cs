using System.Diagnostics.Metrics;

namespace API.Inventory.Model
{
    public class Booking
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public int InventoryId { get; set; }
        public string InventoryName { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingReference { get; set; }
        public string Status { get; set; }
    }
}
