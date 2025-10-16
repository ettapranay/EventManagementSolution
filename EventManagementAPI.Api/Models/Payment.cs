using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventManagementAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [ForeignKey("Booking")]
        public int BookingID { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [MaxLength(50)]
        public string PaymentStatus { get; set; }

        [JsonIgnore]
        public Booking? Booking { get; set; }
    }
}
