using MiniRTLS.API.Models;

namespace MiniRTLS.API.Services
{
    public interface IRabbitMQPublisherService
    {
        void PublishTelemetryReceived(TelemetryReceivedEvent telemetryEvent);
    }
}
