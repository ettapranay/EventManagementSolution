namespace EventManagementMVC.Models
{
    public class ApiLoginResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public LoginResponse data { get; set; }
    }
}
