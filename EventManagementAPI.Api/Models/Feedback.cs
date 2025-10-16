using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventManagementAPI.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }
        [ForeignKey("UserRegistration")]
        public int UserID { get; set; }

        [ForeignKey("EventRegister")]
        public int EventID { get; set; }

        [MaxLength(1000)]
        public string Comments { get; set; }

        public int Rating { get; set; } // Optional 1-5 scale

        [JsonIgnore]
        public UserRegistration? UserRegistration { get; set; }

        [JsonIgnore]
        public EventRegister? EventRegister { get; set; }

    }
}
