using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace EventManagementMVC.Models
{
    public class UserRegistration
    {
        [Key]
        public int UserID { get; set; }

        [Required, MaxLength(150)]
        public string UserName { get; set; }

        [Required, MaxLength(180), EmailAddress]
        public string UserEmail { get; set; }

        [Range(10, 100)]
        public int Age { get; set; }

        [MaxLength(30)]
        public string Gender { get; set; }

        [MaxLength(30), Phone]
        public string PhoneNumber { get; set; }

        [Required, MaxLength(200), DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, MaxLength(200), DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [Range(100000, 999999)]
        public int Pincode { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleID { get; set; }
        [ValidateNever]
        public Role? Role { get; set; } // Reference to the Role model; can be null
    }
}
