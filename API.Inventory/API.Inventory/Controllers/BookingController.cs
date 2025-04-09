using CsvHelper;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using API.Inventory.Model;
using API.Inventory.Repository;

namespace API.Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {

        private readonly IBookingRepository _bookingRepository;

        public BookingController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        // Endpoint to upload bookings from CSV file
        [HttpPost("upload-bookings")]
        public async Task<IActionResult> UploadBookings(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<BookingCsvRecord>().ToList();
                    var result = await _bookingRepository.BookItemAsync(records);
                    if (result)
                    {
                        return Ok("Bookings uploaded successfully.");
                    }

                    return BadRequest("Failed to upload bookings.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to upload bookings.");
            }
        }

        // Endpoint to cancel a booking based on booking reference
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking(int memberId, int bookingId)
        {
            var cancellationResult = await _bookingRepository.CancelBookingAsync(memberId, bookingId);
            if (cancellationResult)
            {
                return Ok("Booking cancelled successfully.");
            }

            return BadRequest("Failed to cancel the booking.");
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBooking()
        {
            try
            {
                var booking = await _bookingRepository.GetAllBookingsAsync();

                if (booking == null)
                {
                    return NotFound($"Booking with  not found.");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
