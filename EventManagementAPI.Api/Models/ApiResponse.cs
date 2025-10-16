namespace EventManagementAPI.Models
{
    public class ApiResponse
    {
        public string Status { get; set; } // "success" or "fail"
        public string Message { get; set; } // human-readable message
        public object Data { get; set; } // any extra data (nullable)

        public ApiResponse(string status, string message, object data = null)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
