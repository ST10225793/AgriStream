using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AgriStream.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(
            IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("BlobStorage")
                ?? throw new InvalidOperationException(
                    "Blob storage connection string missing.");

            string containerName =
                configuration["BlobStorage:ContainerName"]
                ?? "drone-images";

            var blobServiceClient =
                new BlobServiceClient(connectionString);

            _container =
                blobServiceClient.GetBlobContainerClient(
                    containerName);
        }

        public async Task<string> UploadImageAsync(
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No image selected.");

            string extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            var allowedExtensions =
                new[] { ".jpg", ".jpeg", ".png", ".tif", ".tiff" };

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    "Unsupported image format.");

            string blobName =
                $"{Guid.NewGuid()}{extension}";

            BlobClient blobClient =
                _container.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    }
                });

            return blobName;
        }
    }
}