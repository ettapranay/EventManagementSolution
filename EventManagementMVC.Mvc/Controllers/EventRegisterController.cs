using EventManagementMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementMVC.Controllers
{
    public class EventRegisterController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public EventRegisterController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiBaseUrls:EventManagementApi"];
        }

        // ✅ List all events
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl + "EventRegister/AllEvents");
            List<EventRegister> eventsList = new();

            if (response.IsSuccessStatusCode)
            {
                var apiResult = await response.Content.ReadFromJsonAsync<ApiListResult>();
                if (apiResult?.data != null)
                {
                    eventsList = apiResult.data;
                }
            }

            ViewBag.Message = TempData["Message"] as string;
            return View(eventsList);
        }

        // ✅ Details view
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}EventRegister/GetByEventId/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiResult = await response.Content.ReadFromJsonAsync<ApiSingleResult>();
                if (apiResult?.data != null)
                    return View(apiResult.data);
            }
            return NotFound();
        }

        // ✅ Create (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View(new EventRegister());
        }

        // ✅ Create (POST)
        [HttpPost]
        public async Task<IActionResult> Create(EventRegister eventModel)
        {
            if (!ModelState.IsValid)
                return View(eventModel);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(_apiBaseUrl + "EventRegister/AddEvent", eventModel);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Event created successfully!";
                ModelState.Clear();
                return View(new EventRegister());
            }
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                ViewBag.Message = "Failed to create event: " + result;
                return View(eventModel);
            }
        }

        // ✅ Edit (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}EventRegister/GetByEventId/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiResult = await response.Content.ReadFromJsonAsync<ApiSingleResult>();
                if (apiResult?.data != null)
                    return View(apiResult.data);
            }
            return NotFound();
        }

        // ✅ Edit (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(EventRegister eventModel)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"{_apiBaseUrl}EventRegister/UpdateEvent/{eventModel.EventID}", eventModel);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Event updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Message = "Update failed";
            return View(eventModel);
        }

        // ✅ Delete (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}EventRegister/GetByEventId/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiResult = await response.Content.ReadFromJsonAsync<ApiSingleResult>();
                if (apiResult?.data != null)
                    return View(apiResult.data);
            }
            return NotFound();
        }

        // ✅ Delete (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}EventRegister/DeleteEvent/{id}");

            if (response.IsSuccessStatusCode)
                TempData["Message"] = "Event deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Helper classes for deserialization
        public class ApiSingleResult
        {
            public EventRegister data { get; set; }
            public string message { get; set; }
            public string status { get; set; }
        }

        public class ApiListResult
        {
            public List<EventRegister> data { get; set; }
            public string message { get; set; }
            public string status { get; set; }
        }
    }
}
