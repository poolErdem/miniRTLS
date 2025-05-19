using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using MiniRTLS.API.Models;

namespace MiniRTLS.API.Services
{
    public class TelemetryConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TelemetryConsumerService> _logger;
        private IConnection _connection;
        private IModel _channel;

        public TelemetryConsumerService(IServiceProvider serviceProvider, ILogger<TelemetryConsumerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            var factory = new ConnectionFactory() { HostName = "localhost" }; // appsettings.json'dan alabilirsin
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "telemetry_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"Telemetry received from queue: {json}");

                try
                {
                    var telemetry = JsonSerializer.Deserialize<TelemetryReceivedEvent>(json);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();

                        var newTelemetry = new Telemetry
                        {
                            AssetId = telemetry.AssetId,
                            X = telemetry.X,
                            Y = telemetry.Y,
                            Temperature = telemetry.Temperature,
                            Timestamp = telemetry.Timestamp
                        };

                        dbContext.Telemetries.Add(newTelemetry);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing telemetry message");
                }
            };

            _channel.BasicConsume(queue: "telemetry_queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
