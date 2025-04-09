using System.ComponentModel.DataAnnotations.Schema;

namespace API.Inventory.Model
{
    [Table("Members")]
    public class Members
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int BookingCount { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
