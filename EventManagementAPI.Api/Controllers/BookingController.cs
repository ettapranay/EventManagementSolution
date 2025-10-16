using EventManagementAPI.Data;
using EventManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly EventDB _dbContext;

        public BookingController(EventDB dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Booking/AllBookings
        [Authorize(Roles = "Admin,Organizer")]
        [HttpGet("AllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _dbContext.BookingList
                    .Include(b => b.UserRegistration)
                    .Include(b => b.EventRegister)
                    .ToListAsync();
                return Ok(new ApiResponse("success", " All Booking Details had Retrived", bookings));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // GET: api/Booking/GetById/1
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpGet("GetById/{BookingId}")]
        public async Task<IActionResult> GetBookingById(int BookingId)
        {
            try
            {
                var booking = await _dbContext.BookingList
                .Include(b => b.UserRegistration)
                .Include(b => b.EventRegister)
                .FirstOrDefaultAsync(b => b.BookingID == BookingId);

                if (booking == null)
                    return NotFound(new ApiResponse("fail", "Booking not found"));
                return Ok(new ApiResponse("success", "Booking found", booking));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // POST: api/Booking/AddBooking

        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpPost("AddBooking")]
        public async Task<IActionResult> AddBooking([FromBody] Booking booking)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponse("fail", "Invalid model data", ModelState));

                // Optional: validate UserID and EventID existence here if you want

                _dbContext.BookingList.Add(booking);
                var res=await _dbContext.SaveChangesAsync();
                if (res > 0)
                { return Ok(new ApiResponse("success", "Booking is Registered successfully", booking)); }
                else { return BadRequest(new ApiResponse("fail", "Booking is Not Registered")); }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // PUT: api/Booking/UpdateBooking/1

        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpPut("UpdateBooking/{BookingId}")]
        public async Task<IActionResult> UpdateBooking(int BookingId, [FromBody] Booking updateBooking)
        {
            try
            {
                if (BookingId != updateBooking.BookingID)
                    return BadRequest(new ApiResponse("fail", "Given Booking Id does not match with request body"));

                var existingBooking = await _dbContext.BookingList.FindAsync(BookingId);

                if (existingBooking == null)
                    return NotFound(new ApiResponse("fail", "Booking not found"));

                // Update only non-default / non-null fields
                if (updateBooking.UserID != 0)
                    existingBooking.UserID = updateBooking.UserID;

                if (updateBooking.EventID != 0)
                    existingBooking.EventID = updateBooking.EventID;

                if (updateBooking.BookingDate != default)
                    existingBooking.BookingDate = updateBooking.BookingDate;

                if (updateBooking.NoOfSeats != 0)
                    existingBooking.NoOfSeats = updateBooking.NoOfSeats;

                _dbContext.BookingList.Update(existingBooking);
                var res=await _dbContext.SaveChangesAsync();
                if (res > 0)
                {
                    return Ok(new ApiResponse("success", "Booking is Updated successfully", existingBooking));
                }
                else { return BadRequest(new ApiResponse("fail", "Booking is Not Updated")); }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // DELETE: api/Booking/DeleteBooking/1
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpDelete("DeleteBooking/{BookingId}")]
        public async Task<IActionResult> DeleteBooking(int BookingId)
        {
            try
            {
                var booking = await _dbContext.BookingList.FindAsync(BookingId);

                if (booking == null)
                    return NotFound(new ApiResponse("fail", "Booking not found."));

                _dbContext.BookingList.Remove(booking);
                 var res=await _dbContext.SaveChangesAsync();
                if (res > 0)
                {
                    return Ok(new ApiResponse("success", "Booking is  deleted successfully", booking));
                }
                else {
                    return BadRequest(new ApiResponse("fail","Booking is Not Deleted"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }
    }
}
