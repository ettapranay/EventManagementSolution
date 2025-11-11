using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EventManagementMVC.Models;
namespace EventManagementMVC.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class UserRegistrationController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        // You may want to inject roles or fetch them from your API/db. For this example, it's static.
        private List<Role> GetRoles() => new List<Role>
        {
            new Role { RoleID = 1, RoleName = "Admin" },
            new Role { RoleID = 2, RoleName = "Organizer" },
            new Role { RoleID = 3, RoleName = "Customer" }
        };

        public UserRegistrationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            // Reads the URL from appsettings:
            _apiBaseUrl = configuration["ApiBaseUrls:EventManagementApi"];
        }

        // List all users
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl + "UserRegistration/AllDetails");
            List<UserRegistration> usersList = new List<UserRegistration>();

            if (response.IsSuccessStatusCode)
            {
                var apiResult = await response.Content.ReadFromJsonAsync<ApiListResult>();
                if (apiResult != null && apiResult.data != null)
                {
                    usersList = apiResult.data;
                }
            }

            // Show success message if present
            ViewBag.Message = TempData["Message"] as string;

            return View(usersList);
        }

        // View user details
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}UserRegistration/GetByUserId/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiResult = await response.Content.ReadFromJsonAsync<ApiSingleResult>();
                if (apiResult != null && apiResult.data != null)
                    return View(apiResult.data);
            }
            return NotFound();
        }

        // GET: Edit user form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}UserRegistration/GetByUserId/{id}");
            if (response.IsSuccessStatusCode)
            { 

                var apiResult = await response.Content.ReadFromJsonAsync<ApiSingleResult>();
                if (apiResult != null && apiResult.data != null)
                {
                    ViewBag.Roles = new SelectList(GetRoles(), "RoleID", "RoleName", apiResult.data.Role?.RoleID ?? 0);
                    return View(apiResult.data);
                }
            }
            return NotFound();
        }

        // POST: Edit user
        [HttpPost]
        public async Task<IActionResult> Edit(UserRegistration user)
        {
            ViewBag.Roles = new SelectList(GetRoles(), "RoleID", "RoleName", user.Role?.RoleID ?? 0);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"{_apiBaseUrl}UserRegistration/UpdateUser/{user.UserID}", user);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Message = "Update failed";
            return View(user);
        }

        // GET: Delete confirmation
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}UserRegistration/GetByUserId/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiResult = await response.Content.ReadFromJsonAsync<ApiSingleResult>();
                if (apiResult != null && apiResult.data != null)
                    return View(apiResult.data);
            }
            return NotFound();
        }

        // POST: Confirm delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}UserRegistration/DeleteUser/{id}");
            if (response.IsSuccessStatusCode)
                TempData["Message"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserRegistration/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(GetRoles(), "RoleID", "RoleName");
            return View(new UserRegistration());
        }

        // POST: UserRegistration/Create
        [HttpPost]
        public async Task<IActionResult> Create(UserRegistration user)
        {
            ViewBag.Roles = new SelectList(GetRoles(), "RoleID", "RoleName", user.RoleID);

            if (!ModelState.IsValid)
            {
                return View(user);
            }


            user.Role = GetRoles().FirstOrDefault(r => r.RoleID == user.RoleID);
            if (user.RoleID == 0)
            {
                user.RoleID = 3;
            }
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(_apiBaseUrl + "UserRegistration/AddRecord", user);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = " User created successfully!";
                ModelState.Clear();
                return View(new UserRegistration());
            }
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                ViewBag.Message = " Failed to create user: " + result;
                return View(user); // keep entered values if failed
            }
        }

        // Helper classes for API response deserialization
        public class ApiSingleResult
        {
            public UserRegistration data { get; set; }
            public string message { get; set; }
            public string status { get; set; }
        }
        public class ApiListResult
        {
            public List<UserRegistration> data { get; set; }
            public string message { get; set; }
            public string status { get; set; }
        }

    }
}
