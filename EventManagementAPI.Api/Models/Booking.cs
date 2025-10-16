using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventManagementAPI.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        public int UserID { get; set; }


        public int EventID { get; set; }

        public DateTime BookingDate { get; set; }

        public int NoOfSeats { get; set; }

        [JsonIgnore]
        public UserRegistration? UserRegistration { get; set; }
        [JsonIgnore]
        public EventRegister? EventRegister { get; set; }

    }
}
