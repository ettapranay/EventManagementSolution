using EventManagementAPI.Data;
using EventManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly EventDB _dbContext;

        public PaymentController(EventDB dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Payment/AllPayments
        [HttpGet("AllPayments")]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var payments = await _dbContext.PaymentList
                    .Include(p => p.Booking)
                    .ToListAsync();
                return Ok(new ApiResponse("success","Retrieved All Payment",payments));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // GET: api/Payment/GetById/1
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var payment = await _dbContext.PaymentList
                    .Include(p => p.Booking)
                    .FirstOrDefaultAsync(p => p.PaymentID == id);

                if (payment == null)
                    return NotFound("Payment not found.");

                return Ok(new ApiResponse("success","Retrieved Payment",payment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        [HttpPost("AddPayment")]
        public async Task<IActionResult> AddPayment([FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Retrieve the related Booking and EventRegister entities
            // We use .Include() to eagerly load the related EventRegister
            var booking = await _dbContext.BookingList
                                .Include(b => b.EventRegister) // Include the EventRegister linked to the Booking
                                .FirstOrDefaultAsync(b => b.BookingID == payment.BookingID);

            if (booking == null)
            {
                return NotFound(new ApiResponse("fail",$"Booking with ID {payment.BookingID} not found."));
            }

            if (booking.EventRegister == null)
            {
                // This scenario indicates a data integrity issue if EventID is required in Booking
                // and EventRegister doesn't exist for that ID.
                return StatusCode(500,new ApiResponse("fail", $"Associated event for Booking ID {payment.BookingID} not found."));
            }

            // 2. Calculate the Amount
            // Ensure you handle potential decimal precision correctly.
            decimal calculatedAmount = booking.NoOfSeats * booking.EventRegister.TicketPrice;

            // 3. Assign the calculated amount to the payment object
            payment.Amount = calculatedAmount;

            // Optional: You might want to update the booking status to 'Paid' or similar here
            // booking.Status = "Paid"; // Assuming Booking model has a Status property

            // 4. Add the payment and save changes
            _dbContext.PaymentList.Add(payment);
            await _dbContext.SaveChangesAsync();

            return Ok(new ApiResponse("success", "Payment added successfully with calculated amount."));
        }

        // PUT: api/Payment/UpdatePayment/1
        [HttpPut("UpdatePayment/{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment updatePayment)
        {
            if (id != updatePayment.PaymentID)
                return BadRequest(new ApiResponse("fail","PaymentID mismatch."));

            var existingPayment = await _dbContext.PaymentList.FindAsync(id);

            if (existingPayment == null)
                return NotFound(new ApiResponse("fail","Payment not found."));

            if (updatePayment.BookingID != 0)
                existingPayment.BookingID = updatePayment.BookingID;

            if (updatePayment.Amount != 0)
                existingPayment.Amount = updatePayment.Amount;

            if (updatePayment.PaymentDate != default)
                existingPayment.PaymentDate = updatePayment.PaymentDate;

            if (!string.IsNullOrEmpty(updatePayment.PaymentMethod))
                existingPayment.PaymentMethod = updatePayment.PaymentMethod;

            if (!string.IsNullOrEmpty(updatePayment.PaymentStatus))
                existingPayment.PaymentStatus = updatePayment.PaymentStatus;

            _dbContext.PaymentList.Update(existingPayment);
            await _dbContext.SaveChangesAsync();

            return Ok(new ApiResponse("success", "Payment updated successfully."));
        }

        // DELETE: api/Payment/DeletePayment/1
        [HttpDelete("DeletePayment/{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _dbContext.PaymentList.FindAsync(id);

            if (payment == null)
                return NotFound(new ApiResponse("fail", "Payment not found."));

            _dbContext.PaymentList.Remove(payment);
            await _dbContext.SaveChangesAsync();

            return Ok(new ApiResponse("success", "Payment deleted successfully."));
        }

    }
}
