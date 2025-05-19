using System.Text.Json.Serialization;

namespace MiniRTLS.API.Models
{   
    public class TelemetryReceivedEvent
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Temperature { get; set; }
        public int AssetId { get; set; }
        public string? Location { get; set; }
        public DateTime Timestamp { get; set; }
        public double BatteryLevel { get; set; }
        public bool Motion { get; set; }
    }
}
