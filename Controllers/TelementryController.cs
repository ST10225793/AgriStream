
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace AgriStream.Controllers
{
    public class TelemetryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TelemetryController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var functionUrl = _configuration[
                "AzureFunctions:TelemetryGetUrl"];

            if (string.IsNullOrWhiteSpace(functionUrl))
            {
                ViewBag.Error = "Telemetry Function URL is not configured.";
                return View(new List<TelemetryReading>());
            }

            try
            {
                var readings = await _httpClient
                    .GetFromJsonAsync<List<TelemetryReading>>(
                        functionUrl);

                return View(readings ?? new List<TelemetryReading>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to retrieve telemetry: " + ex.Message;
                return View(new List<TelemetryReading>());
            }
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
    }

   

    public class TelemetryReading
    {
        public string? DeviceId { get; set; }
        public string? SoilMoisture { get; set; }
        public string? Temperature { get; set; }
        public string? Humidity { get; set; }
        public string? Timestamp { get; set; }
    }
}
