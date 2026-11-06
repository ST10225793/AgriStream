using Microsoft.AspNetCore.Mvc;
using AgriStream.Services;

namespace AgriStream.Controllers
{
    public class DroneController : Controller
    {
        private readonly BlobStorageService _blobService;

        public DroneController(
            BlobStorageService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(
            IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                ViewBag.Message = "Please select an image.";
                return View();
            }

            try
            {
                string fileName =
                    await _blobService.UploadImageAsync(image);

                ViewBag.Message =
                    "Drone image successfully uploaded to Azure Blob Storage!";

                ViewBag.FileName = fileName;
            }
            catch (Exception ex)
            {
                ViewBag.Message =
                    "Upload failed: " + ex.Message;
            }

            return View();
        }
    }
}