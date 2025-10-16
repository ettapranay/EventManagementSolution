using System.ComponentModel.DataAnnotations;

namespace EventManagementMVC.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
