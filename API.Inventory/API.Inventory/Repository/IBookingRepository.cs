using System.Threading.Tasks;
using API.Inventory.Model;
using API.Inventory.Models;

namespace API.Inventory.Repository
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Members>> GetMembersAsync();
        Task<Members> GetMemberByIdAsync(int id);
        Task<List<InventoryDto>> GetAvailableInventoryAsync();
        Task<bool> BookItemAsync(List<BookingCsvRecord> bookingCsvRecord);
        Task<bool> CancelBookingAsync(int memberId, int bookingId);
        Task<IEnumerable<Model.Booking>> GetAllBookingsAsync();
    }
}
