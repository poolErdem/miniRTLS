using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniRTLS.API.Models;
using MiniRTLS.API.Data;
using Microsoft.AspNetCore.SignalR;
using MiniRTLS.API.Hubs;
using MiniRTLS.API.Services;    // RabbitMQPublisherService namespace'i
using System.Threading.Tasks;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class TelemetryController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<AssetHub> _hubContext;
    private readonly IRabbitMQPublisherService _rabbitMQPublisher;

    public TelemetryController(
        AppDbContext context, 
        IHubContext<AssetHub> hubContext,
        IRabbitMQPublisherService rabbitMQPublisher)
    {
        _context = context;
        _hubContext = hubContext;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    // GET: api/telemetry
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Telemetry>>> GetTelemetries()
    {
        return await _context.Telemetries.ToListAsync();
    }

    // POST: api/telemetry
    [HttpPost]
    public async Task<ActionResult<Telemetry>> PostTelemetry(Telemetry telemetry)
    {
        _context.Telemetries.Add(telemetry);
        await _context.SaveChangesAsync();

        // Son konumu SignalR ile clientlara gönder
        var lastTelemetry = new
        {
            telemetry.AssetId,
            telemetry.X,
            telemetry.Y,
            telemetry.Temperature,
            telemetry.Timestamp
        };
        await _hubContext.Clients.Group(telemetry.AssetId.ToString()).SendAsync("ReceiveTelemetryUpdate", lastTelemetry);

        // Event modelini oluştur ve RabbitMQ ile yayınla
        var telemetryEvent = new TelemetryReceivedEvent
        {
            AssetId = telemetry.AssetId,
            X = telemetry.X,
            Y = telemetry.Y,
            Temperature = telemetry.Temperature,
            Timestamp = telemetry.Timestamp
        };

        _rabbitMQPublisher.PublishTelemetryReceived(telemetryEvent);

        return CreatedAtAction(nameof(GetTelemetries), new { id = telemetry.Id }, telemetry);
    }

    // PUT: api/telemetry/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTelemetry(int id, Telemetry updatedTelemetry)
    {
        if (id != updatedTelemetry.Id)
        {
            return BadRequest("ID mismatch");
        }

        var telemetry = await _context.Telemetries.FindAsync(id);
        if (telemetry == null)
        {
            return NotFound();
        }

        telemetry.X = updatedTelemetry.X;
        telemetry.Y = updatedTelemetry.Y;
        telemetry.Temperature = updatedTelemetry.Temperature;
        telemetry.Timestamp = updatedTelemetry.Timestamp;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Telemetries.Any(t => t.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }
}
