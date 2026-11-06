
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace AgriStream.Controllers
{
    public class IrrigationController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IrrigationController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Trigger(
            string deviceId,
            int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(deviceId) ||
                durationMinutes < 1 ||
                durationMinutes > 120)
            {
                TempData["Error"] = "Please enter valid irrigation details.";
                return RedirectToAction("Index");
            }

            var functionUrl = _configuration[
                "AzureFunctions:IrrigationPostUrl"];

            if (string.IsNullOrWhiteSpace(functionUrl))
            {
                TempData["Error"] = "Irrigation Function URL is not configured.";
                return RedirectToAction("Index");
            }

            try
            {
                var command = new IrrigationCommand
                {
                    DeviceId = deviceId,
                    DurationMinutes = durationMinutes
                };

                var response = await _httpClient.PostAsJsonAsync(
                    functionUrl, command);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] =
                        "Irrigation command successfully submitted.";
                }
                else
                {
                    TempData["Error"] =
                        "The Azure Function rejected the command: " +
                        (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Unable to contact Azure Function: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }

    public class IrrigationCommand
    {
        public string DeviceId { get; set; } = "";
        public int DurationMinutes { get; set; }
    }
}
