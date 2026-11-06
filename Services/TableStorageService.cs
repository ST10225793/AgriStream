
using Azure.Data.Tables;
using AgriStream.Models;

namespace AgriStream.Services
{
    public class TableStorageService
    {
        private readonly TableClient _tableClient;

        public TableStorageService(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("BlobStorage")
                ?? throw new InvalidOperationException(
                    "Table Storage connection string missing.");

            var serviceClient =
                new TableServiceClient(connectionString);

            _tableClient =
                serviceClient.GetTableClient("SoilTelemetry");
        }

        // Save a sensor reading
        public async Task AddTelemetryAsync(
            SoilTelemetry telemetry)
        {
            await _tableClient.CreateIfNotExistsAsync();

            DateTimeOffset timestamp = DateTimeOffset.UtcNow;

            telemetry.PartitionKey =
                $"{telemetry.SensorID}_{timestamp:yyyyMMdd}";

            telemetry.RowKey =
                $"{timestamp.UtcDateTime.Ticks:D19}_{Guid.NewGuid():N}";

            await _tableClient.AddEntityAsync(telemetry);
        }

        // Retrieve readings for a particular sensor and date
        public async Task<List<SoilTelemetry>> GetTelemetryAsync(
            string sensorID,
            DateTime date)
        {
            string partitionKey =
                $"{sensorID}_{date:yyyyMMdd}";

            var results = new List<SoilTelemetry>();

            await foreach (
                var item in _tableClient.QueryAsync<SoilTelemetry>(
                    entity => entity.PartitionKey == partitionKey))
            {
                results.Add(item);
            }

            return results;
        }
    }
}
