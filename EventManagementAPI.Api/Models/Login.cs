using System.ComponentModel.DataAnnotations;

namespace EventManagementAPI.Models
{
    public class Login
    {
        [Required, EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
