using EventManagementAPI.Data;
using EventManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventRegisterController : ControllerBase
    {
        private readonly EventDB DbContext;

        public EventRegisterController(EventDB dbContext)
        {
            DbContext = dbContext;
        }
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpGet("AllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = DbContext.EventRegisterList.Include(e => e.UserRegistration);
                return Ok(new ApiResponse("success", "All Events Retrived successfully", events));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent([FromBody] EventRegister eventModel)
        {
            try
            {
                DbContext.EventRegisterList.Add(eventModel);
                var res = await DbContext.SaveChangesAsync();
                if (res > 0)
                    return Ok(new ApiResponse("success", "EventRegister Inserted successfully", eventModel));
                else
                    return BadRequest(new ApiResponse("fail", "EventRegister not Inserted successfully", eventModel));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpGet("GetByEventId/{EventId}")]
        public async Task<IActionResult> GetByEventId(int EventId)
        {
            try
            {
                var result = DbContext.EventRegisterList
        .Include(e => e.UserRegistration)
        .Where(e => e.EventID == EventId);
                return Ok(new ApiResponse("success", "All Events Retrived successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error:{ex.Message}"));
            }

        }

        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpPut("UpdateEvent/{EventId}")]
        public async Task<IActionResult> UpdateEvent(int EventId, [FromBody] EventRegister updatedEvent)
        {
            string result = string.Empty;
            try
            {
                if (EventId != updatedEvent.EventID)
                    return BadRequest(new ApiResponse("fail", "Given ID does not match"));

                var existingEvent = await DbContext.EventRegisterList
                    .SingleOrDefaultAsync(e => e.EventID == EventId);

                if (existingEvent != null)
                {
                    if (updatedEvent.UserID != 0)
                        existingEvent.UserID = updatedEvent.UserID;

                    if (!string.IsNullOrEmpty(updatedEvent.EventName))
                        existingEvent.EventName = updatedEvent.EventName;

                    if (!string.IsNullOrEmpty(updatedEvent.EventDescription))
                        existingEvent.EventDescription = updatedEvent.EventDescription;

                    if (updatedEvent.EventDate != default)
                        existingEvent.EventDate = updatedEvent.EventDate;

                    if (updatedEvent.EventTime != default)
                        existingEvent.EventTime = updatedEvent.EventTime;

                    if (updatedEvent.EventEndTime != default)
                        existingEvent.EventEndTime = updatedEvent.EventEndTime;

                    if (!string.IsNullOrEmpty(updatedEvent.Location))
                        existingEvent.Location = updatedEvent.Location;

                    if (updatedEvent.TotalSeats != 0)
                        existingEvent.TotalSeats = updatedEvent.TotalSeats;

                    if (updatedEvent.TicketPrice != 0)
                        existingEvent.TicketPrice = updatedEvent.TicketPrice;

                    if (!string.IsNullOrEmpty(updatedEvent.Status))
                        existingEvent.Status = updatedEvent.Status;

                    DbContext.EventRegisterList.Update(existingEvent);
                    var res = await DbContext.SaveChangesAsync();

                    if (res > 0)
                    {
                        return Ok(new ApiResponse("success", "Event Register is Updated Successfully"));
                    }
                    else {
                        return BadRequest(new ApiResponse("fail","Event Register is Not Updated Successfully"));
                    }
                }
                else
                {
                    return BadRequest(new ApiResponse("fail","Event Register is Not Exits To update"));   
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error:{ex.Message}"));
            }
        }

        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpDelete("DeleteEvent/{EventId}")]
        public async Task<IActionResult> DeleteEvent(int EventId)
        {
            string result = string.Empty;
            try
            {
                var existing = await DbContext.EventRegisterList
                    .SingleOrDefaultAsync(e => e.EventID == EventId);

                if (existing != null)
                {
                    DbContext.EventRegisterList.Remove(existing);
                    var res = await DbContext.SaveChangesAsync();
                    if (res > 0)
                    {
                        return Ok(new ApiResponse("success", "Event Register is Deleted Successfully"));
                    }
                    else {
                        return BadRequest(new ApiResponse("fail","Event Register is Not Deleted Successfully"));
                    }
                }
                else
                {
                    return BadRequest(new ApiResponse("fail","Event Register not exits to Delete"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500,new ApiResponse("fail", $"Error:{ex.Message}"));
            }
        }
    }
}
