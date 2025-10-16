using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventManagementAPI.Models
{
    public class EventOrganize
    {
        [Key]
        public int OrganizeID { get; set; }

        public int OrganizerID { get; set; }
        [ForeignKey("OrganizerID")] // This is helpful for clarity, though HasForeignKey in fluent API also works.

        [JsonIgnore]
        public UserRegistration? UserRegistration { get; set; } // Add this navigation property

        public int EventID { get; set; }
        [ForeignKey("EventID")] // This is helpful for clarity

        [JsonIgnore]
        public EventRegister? EventRegister { get; set; } // Add this navigation property
        [Required]
        [MaxLength(200)]
        public string RoleDescription { get; set; }

        //public UserRegistration UserRegistration { get; set; }
        //public EventRegister EventRegister { get; set; }
    }
}
