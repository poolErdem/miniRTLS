using System.Text.Json.Serialization;

namespace MiniRTLS.API.Models
{
    public class Telemetry
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Temperature { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonIgnore] // Newtonsoft.Json kullanıyorsan
        public Asset? Asset { get; set; }
    }
}
