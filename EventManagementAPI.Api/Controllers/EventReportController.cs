

using EventManagementAPI.Data;
using EventManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventReportController : ControllerBase
    {
        private readonly EventDB _dbContext;

        public EventReportController(EventDB dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("GetByEventNameWithRevenue")]
        public async Task<IActionResult> GetByEventNameWithRevenue([FromQuery] string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return BadRequest(new ApiResponse("fail", "Event name cannot be empty."));
            }

            try
            {
                var result = await _dbContext.EventRegisterList
                    .Where(e => e.EventName.Contains(eventName))
                    .Select(e => new
                    {
                        e.EventID,
                        e.EventName,
                        e.EventDescription,
                        e.EventDate,
                        e.EventTime,
                        e.EventEndTime,
                        e.Location,
                        e.TotalSeats,
                        e.TicketPrice,
                        e.Status,
                        TotalRevenue = _dbContext.BookingList
                                                 .Where(b => b.EventID == e.EventID)
                                                 .Join(_dbContext.PaymentList,
                                                       booking => booking.BookingID,
                                                       payment => payment.BookingID,
                                                       (booking, payment) => payment.Amount)
                                                 .Sum(), // Sum here. If no payments, it will implicitly be 0 for decimal.
                        BookedSeats = _dbContext.BookingList
                                                .Where(b => b.EventID == e.EventID)
                                                .Sum(b => (int?)b.NoOfSeats) ?? 0, // Sum nullable int to handle no bookings
                        EventReports = _dbContext.EventReportsList
                                                 .Where(er => er.EventID == e.EventID)
                                                 .Select(er => new
                                                 {
                                                     er.EventReportID,
                                                     er.EventID,
                                                     er.ReportDetails,
                                                     er.SubmittedDate
                                                 })
                                                 .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    return NotFound(new ApiResponse("fail", $"Event with name '{eventName}' not found."));
                }

                // Calculate remaining seats after fetching the result
                var remainingSeats = result.TotalSeats - result.BookedSeats;
                var response = new
                {
                    result.EventID,
                    result.EventName,
                    result.EventDescription,
                    result.EventDate,
                    result.EventTime,
                    result.EventEndTime,
                    result.Location,
                    result.TotalSeats, // Total Seats (Seats Available)
                    result.TicketPrice,
                    result.Status,
                    result.TotalRevenue,
                    RemainingSeats = remainingSeats, // Calculated remaining seats
                    result.EventReports
                };
                // Return a new anonymous object with all desired properties
                return Ok(new ApiResponse("Success", "Retrieved Successfully Event Name With Revenue", response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }


        [HttpGet("GetAllEventsWithTotalRevenue")]
        public async Task<IActionResult> GetAllEventsWithTotalRevenue()
        {
            try
            {
                var eventsData = await _dbContext.EventRegisterList
                    .GroupJoin( // Left join EventRegister with the aggregated booking/payment data
                        _dbContext.BookingList
                            .Join(
                                _dbContext.PaymentList,
                                booking => booking.BookingID,
                                payment => payment.BookingID,
                                // Result of join: anonymous type with EventID, NoOfSeats, and Amount
                                (booking, payment) => new { booking.EventID, booking.NoOfSeats, payment.Amount }
                            )
                            .GroupBy(bp => bp.EventID) // Group by EventID
                            .Select(g => new // Calculate sum per event
                            {
                                EventID = g.Key,
                                TotalRevenue = g.Sum(x => (decimal?)x.Amount) ?? 0m,
                                TotalBookedSeats = g.Sum(x => (int?)x.NoOfSeats) ?? 0
                            }),
                        eventRegister => eventRegister.EventID, // Key for EventRegister
                        eventAggregates => eventAggregates.EventID, // Key for the aggregated data
                        (eventRegister, eventAggregates) => new { eventRegister, eventAggregates }
                    )
                    .SelectMany( // Flatten the grouped results
                        x => x.eventAggregates.DefaultIfEmpty(), // Use DefaultIfEmpty to ensure left join behavior
                        (eventRegisterAndAggregates, aggregate) => new // Project into the final desired shape
                        {
                            eventRegisterAndAggregates.eventRegister.EventID,
                            eventRegisterAndAggregates.eventRegister.EventName,
                            eventRegisterAndAggregates.eventRegister.EventDescription,
                            eventRegisterAndAggregates.eventRegister.EventDate,
                            eventRegisterAndAggregates.eventRegister.EventTime,
                            eventRegisterAndAggregates.eventRegister.EventEndTime,
                            eventRegisterAndAggregates.eventRegister.Location,
                            eventRegisterAndAggregates.eventRegister.TotalSeats, // Total Seats (Seats Available)
                            eventRegisterAndAggregates.eventRegister.TicketPrice,
                            eventRegisterAndAggregates.eventRegister.Status,
                            TotalRevenue = aggregate != null ? aggregate.TotalRevenue : 0m,
                            BookedSeats = aggregate != null ? aggregate.TotalBookedSeats : 0,
                            RemainingSeats = eventRegisterAndAggregates.eventRegister.TotalSeats - (aggregate != null ? aggregate.TotalBookedSeats : 0) // Calculated remaining seats
                        }
                    )
                    .ToListAsync();


                if (!eventsData.Any())
                {
                    return NotFound(new ApiResponse("fail", "No events found."));
                }

                return Ok(new ApiResponse("Sucsess", "Retrieved All Events With Total Revenue", eventsData));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }
    }
}
