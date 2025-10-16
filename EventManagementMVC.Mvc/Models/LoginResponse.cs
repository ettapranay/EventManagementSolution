namespace EventManagementMVC.Models
{
    public class LoginResponse
    {
        public string token { get; set; }
        public int userId { get; set; }
        public string role { get; set; }
    }
}
