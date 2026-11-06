
using Microsoft.AspNetCore.Mvc;
using AgriStream.Models;
using AgriStream.Services;

namespace AgriStream.Controllers
{
    public class TelemetryController : Controller
    {
        private readonly TableStorageService _tableService;

        public TelemetryController(
            TableStorageService tableService)
        {
            _tableService = tableService;
        }

        // Display telemetry input form
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // Save telemetry reading
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            SoilTelemetry telemetry)
        {
            if (!ModelState.IsValid)
            {
                return View(telemetry);
            }

            try
            {
                await _tableService.AddTelemetryAsync(telemetry);

                TempData["Success"] =
                    "Telemetry successfully saved to Azure Table Storage!";

                return RedirectToAction(nameof(Add));
            }
            catch (Exception ex)
            {
                // Log the error rather than displaying sensitive details
                return StatusCode(500,
                    "Unable to save telemetry. Check the application logs.");
            }
        }

        // Display stored telemetry readings
        [HttpGet]
        public async Task<IActionResult> ViewTelemetry(
            string sensorID,
            DateTime? date)
        {
            if (string.IsNullOrWhiteSpace(sensorID))
            {
                return BadRequest("Sensor ID is required.");
            }

            try
            {
                var records =
                    await _tableService.GetTelemetryAsync(
                        sensorID,
                        date ?? DateTime.UtcNow.Date);

                ViewBag.SensorID = sensorID;

                return View(records);
            }
            catch
            {
                return StatusCode(500,
                    "Unable to retrieve telemetry records.");
            }
        }
    }
}
