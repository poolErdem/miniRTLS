using Microsoft.EntityFrameworkCore;
using MiniRTLS.API.Data;
using MiniRTLS.API.Hubs;
using MiniRTLS.API.Services;
using RabbitMQ.Client;  

var builder = WebApplication.CreateBuilder(args);

// DB bağlan
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IRabbitMQPublisherService, RabbitMQPublisherService>();
builder.Services.AddHostedService<TelemetryConsumerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AssetHub>("/hubs/asset");  // Hub endpoint

app.Run();
