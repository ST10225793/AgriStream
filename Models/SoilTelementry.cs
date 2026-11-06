
using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace AgriStream.Models
{
    public class SoilTelemetry : ITableEntity
    {
        public string PartitionKey { get; set; } = "";

        public string RowKey { get; set; } = "";

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        [Required]
        public string SensorID { get; set; } = "";

        [Range(0, 100)]
        public double SoilMoisture { get; set; }

        [Range(-50, 80)]
        public double Temperature { get; set; }

        [Range(0, 14)]
        public double SoilPH { get; set; }

        [Required]
        public string Location { get; set; } = "";
    }
}
