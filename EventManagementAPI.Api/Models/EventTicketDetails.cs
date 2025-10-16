namespace EventManagementAPI.Models
{
    public class EventTicketDetails
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfTickets { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public string PaymentStatus { get; set; }

        public int AttendeeUserId { get; set; }
        public string AttendeeName { get; set; }
        public string AttendeeEmail { get; set; }
        public string AttendeePhone { get; set; }

        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }
        public string EventLocation { get; set; }
        public string EventCategory { get; set; }
    }
}
