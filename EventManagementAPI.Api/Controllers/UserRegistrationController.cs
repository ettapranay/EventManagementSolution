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
    public class UserRegistrationController : ControllerBase
    {
        private readonly EventDB _dbContext;

        public UserRegistrationController(EventDB dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/UserRegistration/AllDetails
        [Authorize(Roles = "Admin")]
        [HttpGet("AllDetails")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _dbContext.UserRegisterationList.ToListAsync();
                return Ok(new ApiResponse("success", "Users fetched successfully", users));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // GET: api/UserRegistration/GetByUserId/1
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpGet("GetByUserId/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _dbContext.UserRegisterationList.FindAsync(userId);
                if (user == null)
                    return NotFound(new ApiResponse("fail", "User not found"));

                return Ok(new ApiResponse("success", "User found", user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }
        // POST: api/UserRegistration/AddRecord
        [AllowAnonymous]
        [HttpPost("AddRecord")]
        public async Task<IActionResult> AddUser([FromBody] UserRegistration user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponse("fail", "Invalid model data", ModelState));

                if (user.Password != user.ConfirmPassword)
                    return BadRequest(new ApiResponse("fail", "Password and Confirm Password must match."));
                if (user.Role == null)
                    user.Role = new Role();

                // Assign RoleName based on RoleID
                switch (user.Role.RoleID)
                {
                    case 1:
                        user.Role.RoleName = "Admin";
                        break;
                    case 2:
                        user.Role.RoleName = "Organizer";
                        break;
                    default:
                        user.Role.RoleID = 3;
                        user.Role.RoleName = "Customer";
                        break;
                }

                _dbContext.UserRegisterationList.Add(user);
                await _dbContext.SaveChangesAsync();

                return Ok(new ApiResponse("success", "User registered successfully", user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        //  PUT: api/UserRegistration/UpdateUser/1
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpPut("UpdateUser/{UserId}")]
        public async Task<IActionResult> UpdateUser(int UserId, [FromBody] UserRegistration updateUser)
        {
            try
            {
                if (UserId != updateUser.UserID)
                {
                    return BadRequest(new ApiResponse("fail", "Given Id does not match with request body"));
                }

                var existingUser = await _dbContext.UserRegisterationList.SingleOrDefaultAsync(x => x.UserID == UserId);
                if (existingUser == null)
                    return NotFound(new ApiResponse("fail", "User not found"));
                if (!string.IsNullOrEmpty(updateUser.UserName))
                {
                    existingUser.UserName = updateUser.UserName;
                }

                if (!string.IsNullOrEmpty(updateUser.UserEmail))
                {
                    existingUser.UserEmail = updateUser.UserEmail;
                }

                if (updateUser.Age != 0)
                {
                    existingUser.Age = updateUser.Age;
                }

                if (!string.IsNullOrEmpty(updateUser.Gender))
                {
                    existingUser.Gender = updateUser.Gender;
                }

                if (!string.IsNullOrEmpty(updateUser.PhoneNumber))
                {
                    existingUser.PhoneNumber = updateUser.PhoneNumber;
                }

                if (!string.IsNullOrEmpty(updateUser.Password))
                {
                    existingUser.Password = updateUser.Password;
                }

                if (!string.IsNullOrEmpty(updateUser.ConfirmPassword))
                {
                    existingUser.ConfirmPassword = updateUser.ConfirmPassword;
                }

                if (!string.IsNullOrEmpty(updateUser.Address))
                {
                    existingUser.Address = updateUser.Address;
                }

                if (!string.IsNullOrEmpty(updateUser.City))
                {
                    existingUser.City = updateUser.City;
                }

                if (!string.IsNullOrEmpty(updateUser.State))
                {
                    existingUser.State = updateUser.State;
                }

                if (updateUser.Pincode != 0)
                {
                    existingUser.Pincode = updateUser.Pincode;
                }
                if (updateUser.Role != null) // Check if role data was sent in the body
                {
                    existingUser.Role.RoleID = updateUser.Role.RoleID;
                    existingUser.Role.RoleName = updateUser.Role.RoleName;
                }
                if (updateUser.Role.RoleID == 1)
                {
                    existingUser.Role.RoleID = 1;
                    existingUser.Role.RoleName = "Admin";


                }
                else if (updateUser.Role.RoleID == 2)
                {
                    existingUser.Role.RoleID = 2;
                    existingUser.Role.RoleName = "Organizer";

                }
                else if (updateUser.Role.RoleID == 3 || updateUser.Role.RoleID == 0)
                {
                    existingUser.Role.RoleID = 3;
                    existingUser.Role.RoleName = "Customer";

                }
                _dbContext.UserRegisterationList.Update(existingUser);

                 await _dbContext.SaveChangesAsync();
                return Ok(new ApiResponse("success", "User updated successfully", existingUser));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }

        // DELETE: api/UserRegistration/DeleteUser/1
        [Authorize(Roles = "Admin,Customer,Organizer")]
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _dbContext.UserRegisterationList.FindAsync(userId);
            if (user == null)
                return NotFound(new ApiResponse("fail", "User not found"));

            _dbContext.UserRegisterationList.Remove(user);
            await _dbContext.SaveChangesAsync();

                return Ok(new ApiResponse("success", "User deleted successfully", user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse("fail", $"Error: {ex.Message}"));
            }
        }
    }
}
