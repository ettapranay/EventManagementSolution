using EventManagementMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
namespace EventManagementMVC.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _apiBaseUrl;
        public LoginController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {

            // Reads the URL from appsettings:
            _apiBaseUrl = configuration["ApiBaseUrls:EventManagementApi"];
            _clientFactory = clientFactory;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        // POST: Login
        //[HttpPost]
        //public async Task<ActionResult> Login(Login login)
        //{
        //    if (!ModelState.IsValid)
        //        return View(login);
        //    var client = _clientFactory.CreateClient(); // Create HttpClient here
        //    var postUrl = _apiBaseUrl + "Login/Login";  // final URL: https://localhost:7091/api/Login/Login
        //    var response = await client.PostAsJsonAsync(postUrl, login);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        //        // Save token, role, userId in Session
        //        HttpContext.Session.SetString("JwtToken", result.token.ToString());
        //        HttpContext.Session.SetString("Role", result.role.ToString());
        //        HttpContext.Session.SetString("UserID", result.userId.ToString());

        //        TempData["Message"] = "Login successful!";
        //        return RedirectToAction("Index", "Home");
        //    }
        //    ModelState.AddModelError("", "Invalid email or password.");
        //    return View(login);
        //}
        [HttpPost]
        public async Task<ActionResult> Login(Login login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var client = _clientFactory.CreateClient();
            var postUrl = _apiBaseUrl.TrimEnd('/') + "/Login/Login";

            var json = JsonSerializer.Serialize(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(postUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiLoginResponse>();

                if (apiResponse?.data != null)
                {
                    HttpContext.Session.SetString("JwtToken", apiResponse.data.token ?? "");
                    HttpContext.Session.SetString("Role", apiResponse.data.role ?? "");
                    HttpContext.Session.SetString("UserID", apiResponse.data.userId.ToString());

                    TempData["Message"] = "Login successful!";
                    return RedirectToAction("Index", "EventRegister");
                }

                ModelState.AddModelError("", "Invalid login response.");
                return View(login);
            }

            var respText = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Invalid email or password. {respText}");
            return View(login);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
