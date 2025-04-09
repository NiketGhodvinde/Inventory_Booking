namespace API.Inventory.Model
{
    public class BookingCsvRecord
    {
        public int MemberId { get; set; }
        public int InventoryId { get; set; }
        public string BookingStatus { get; set; }
    }
}
