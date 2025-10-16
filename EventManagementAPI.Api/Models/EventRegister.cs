using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventManagementAPI.Models
{
    public class EventRegister
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        [ForeignKey("UserRegistration")]
        public int UserID { get; set; }

        [Required, MaxLength(50)]
        public string EventName { get; set; }

        [MaxLength(250)]
        public string? EventDescription { get; set; }

        [Required]
        public DateOnly EventDate { get; set; }

        [Required]
        public TimeSpan EventTime { get; set; }

        [Required]
        public TimeSpan EventEndTime { get; set; }

        [MaxLength(250)]
        public string Location { get; set; }

        [Range(1, 100000)]
        public int TotalSeats { get; set; }

        [Precision(18, 2)]
        public decimal TicketPrice { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } // Scheduled, Cancelled, Completed

        // Navigation Property
        [JsonIgnore]
        public UserRegistration? UserRegistration { get; set; }
    }
}
