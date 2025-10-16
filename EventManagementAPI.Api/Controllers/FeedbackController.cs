using EventManagementAPI.Data;
using EventManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly EventDB _dbContext;

        public FeedbackController(EventDB dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Feedback/AllFeedbacks
        [HttpGet("AllFeedbacks")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _dbContext.FeedbackList
                    .Include(f => f.UserRegistration)
                    .Include(f => f.EventRegister)
                    .ToListAsync();
                return Ok(new ApiResponse("success","Retrieved All FeedBacks", feedbacks) );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // GET: api/Feedback/GetById/1
        [HttpGet("GetById/{FeedbackId}")]
        public async Task<IActionResult> GetFeedbackById(int FeedbackId)
        {
            try
            {
                var feedback = await _dbContext.FeedbackList
                    .Include(f => f.UserRegistration)
                    .Include(f => f.EventRegister)
                    .FirstOrDefaultAsync(f => f.FeedbackID == FeedbackId);

                if (feedback == null)
                    return NotFound(new ApiResponse("fail","Feedback not found."));

                return Ok(new ApiResponse("success",$"Retrieved FeedBack by {FeedbackId}",feedback));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // POST: api/Feedback/AddFeedback
        [HttpPost("AddFeedback")]
        public async Task<IActionResult> AddFeedback([FromBody] Feedback feedback)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Optional: Validate UserID and EventID existence if needed

            // Validate rating range if you want (1-5)
            if (feedback.Rating < 1 || feedback.Rating > 5)
                return BadRequest(new ApiResponse("fail","Rating must be between 1 and 5."));

            try
            {
                _dbContext.FeedbackList.Add(feedback);
                await _dbContext.SaveChangesAsync();

                return Ok(new ApiResponse("success","Feedback added successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // PUT: api/Feedback/UpdateFeedback/1
        [HttpPut("UpdateFeedback/{FeedbackId}")]
        public async Task<IActionResult> UpdateFeedback(int FeedbackId, [FromBody] Feedback updateFeedback)
        {
            if (FeedbackId != updateFeedback.FeedbackID)
                return BadRequest(new ApiResponse("fail", "FeedbackID mismatch."));

            var existingFeedback = await _dbContext.FeedbackList.FindAsync(FeedbackId);

            if (existingFeedback == null)
                return NotFound(new ApiResponse("fail", "Feedback not found."));

            if (updateFeedback.UserID != 0)
                existingFeedback.UserID = updateFeedback.UserID;

            if (updateFeedback.EventID != 0)
                existingFeedback.EventID = updateFeedback.EventID;

            if (!string.IsNullOrEmpty(updateFeedback.Comments))
                existingFeedback.Comments = updateFeedback.Comments;

            if (updateFeedback.Rating >= 1 && updateFeedback.Rating <= 5)
                existingFeedback.Rating = updateFeedback.Rating;
            else if (updateFeedback.Rating != 0)
                return BadRequest(new ApiResponse("fail", "Rating must be between 1 and 5."));
            try
            {
                _dbContext.FeedbackList.Update(existingFeedback);
                await _dbContext.SaveChangesAsync();
                return Ok(new ApiResponse("success", "Feedback updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }

        }

        // DELETE: api/Feedback/DeleteFeedback/1
        [HttpDelete("DeleteFeedback/{FeedbackId}")]
        public async Task<IActionResult> DeleteFeedback(int FeedbackId)
        {
            var feedback = await _dbContext.FeedbackList.FindAsync(FeedbackId);

            if (feedback == null)
                return NotFound(new ApiResponse("fail", "Feedback not found."));

            try
            {
                _dbContext.FeedbackList.Remove(feedback);
                await _dbContext.SaveChangesAsync();

                return Ok(new ApiResponse("success", "Feedback deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }
    }
}
