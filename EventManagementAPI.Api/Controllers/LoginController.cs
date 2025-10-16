using Azure.Identity;
using EventManagementAPI.Data;
using EventManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly EventDB _context;
        private readonly IConfiguration _configuration;

        public LoginController(EventDB context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var user = await _context.UserRegisterationList
                .FirstOrDefaultAsync(u => u.UserEmail == login.UserEmail && u.Password == login.Password);

            if (user == null)
                return Unauthorized(new ApiResponse("fail", "Invalid credentials."));

            // Generate JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role?.RoleName),
                new Claim("UserID", user.UserID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EventManagementEventManagementEventManagementEventManagement"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7058",
                audience: "https://localhost:7058",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var TokenData = new {
                token = jwtToken,
                userId = user.UserID,
                role = user.Role?.RoleName ?? "User"
            };
            return Ok(new ApiResponse("success","User Login Successfully",TokenData)
           );
        }
    }
}
