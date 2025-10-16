using EventManagementAPI.Data;
using EventManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventOraganizeController : ControllerBase
    {
        private readonly EventDB DbContext;

        public EventOraganizeController(EventDB dbContext)
        {
            DbContext = dbContext;
        }
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpGet("AllDetails")]
        public async Task<IActionResult> GetAllOrganizedEvents() // Make the method async and return Task<IActionResult>
        {
            try
            {
                var result = await DbContext.EventOrganizeList.ToListAsync(); // AWAIT the asynchronous call
                return Ok(new ApiResponse("success", "Organized Events fetched successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error:{ex.Message}"));
            }
        }
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpPost("AddRecord")]
        public async Task<IActionResult> AddEventOrganize([FromBody] EventOrganize model)
        {
            string result = string.Empty;
            try
            {
                DbContext.EventOrganizeList.Add(model);
                var res=await DbContext.SaveChangesAsync();
                if (res > 0) { return Ok(new ApiResponse("success", "EventOrganize registered successfully", model)); }
                else { return BadRequest(new ApiResponse("fail", "EventOrganize is not registered successfully")); }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpGet("GetByOrganizeId/{OrganizeID}")]
        public async Task<IActionResult> GetByOrganizeId(int OrganizeID)
        {
            try
            {
                var data = DbContext.EventOrganizeList
                    .Where(x => x.OrganizeID == OrganizeID);
                return Ok(new ApiResponse("success", "EventOrganize found", data));
            }
            catch (Exception ex) {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpPut("UpdateRecord/{OrganizeID}")]
        public async Task<IActionResult> UpdateEventOrganize(int OrganizeID, [FromBody] EventOrganize updatedModel)
        {
            try
            {
                if (OrganizeID != updatedModel.OrganizeID)
                    return BadRequest(new ApiResponse("fail", "Given ID does not match with model", ModelState));

                var existingData = await DbContext.EventOrganizeList.SingleOrDefaultAsync(x => x.OrganizeID == OrganizeID);

                if (existingData != null)
                {
                    if (updatedModel.OrganizerID != 0)
                        existingData.OrganizerID = updatedModel.OrganizerID;

                    if (updatedModel.EventID != 0)
                        existingData.EventID = updatedModel.EventID;

                    if (!string.IsNullOrEmpty(updatedModel.RoleDescription))
                        existingData.RoleDescription = updatedModel.RoleDescription;

                    DbContext.EventOrganizeList.Update(existingData);
                    var res=await DbContext.SaveChangesAsync();
                    if (res > 0) { return Ok(new ApiResponse("success", "EventOrgnize is updated successfully", existingData)); }
                    else { return BadRequest(new ApiResponse("fail","EventOrganize is not updated Successfully")); }
                }
                else
                {
                    return NotFound(new ApiResponse("fail", "EventOrganize data not found"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpDelete("DeleteRecord/{OrganizeID}")]
        public async Task<IActionResult> DeleteEventOrganize(int OrganizeID)
        {
            try
            {
                var existing = await DbContext.EventOrganizeList.SingleOrDefaultAsync(x => x.OrganizeID == OrganizeID);

                if (existing != null)
                {
                    DbContext.EventOrganizeList.Remove(existing);
                    var res = await DbContext.SaveChangesAsync();
                    if (res > 0) { return Ok(new ApiResponse("success", "EventOrganize deleted successfully", existing)); }
                    else { return BadRequest(new ApiResponse("fail", "EventOrganize is not Deleted successfully")); }
                }
                else
                {
                    return NotFound(new ApiResponse("fail", "EventOrganize data not found"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }
    }
}
