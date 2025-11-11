namespace EventManagementMVC.Models
{
    public class EventRegister
    {
        public int EventID { get; set; }
        public int UserID { get; set; }
        public string? EventName { get; set; }
        public string? EventDescription { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }
        public TimeSpan EventEndTime { get; set; }
        public string? Location { get; set; }
        public int TotalSeats { get; set; }
        public decimal TicketPrice { get; set; }
        public string? Status { get; set; }
        public UserRegistration? UserRegistration { get; set; }
    }
}
