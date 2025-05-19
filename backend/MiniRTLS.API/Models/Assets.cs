using System.Text.Json.Serialization;

namespace MiniRTLS.API.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;  // örn: Worker, Forklift
        public string Status { get; set; } = string.Empty;  // örn: Active, Idle, Error

        //[JsonIgnore] // Newtonsoft.Json kullanıyorsan
        public List<Telemetry> Telemetries { get; set; } = new();
    }
}
