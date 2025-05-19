using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using MiniRTLS.API.Models;
using MiniRTLS.API.Services;

namespace MiniRTLS.API.Services
{
    public class RabbitMQPublisherService : IRabbitMQPublisherService
    {
        private readonly IModel _channel;
        private readonly string _queueName = "telemetry-events";

        public RabbitMQPublisherService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void PublishTelemetryReceived(TelemetryReceivedEvent telemetryEvent)
        {
            var message = JsonSerializer.Serialize(telemetryEvent);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
