using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventManagementAPI.Models
{
    public class EventReport
    {

        [Key]
        public int EventReportID { get; set; }

        [ForeignKey("EventRegister")]
        public int EventID { get; set; }

        [MaxLength(100)]
        public string? ReportDetails { get; set; }

        public DateTime? SubmittedDate { get; set; }

        [JsonIgnore]
        public EventRegister? EventRegister { get; set; }
    }
}
