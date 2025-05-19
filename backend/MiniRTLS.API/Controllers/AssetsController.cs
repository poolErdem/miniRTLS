using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniRTLS.API.Data;
using MiniRTLS.API.Models;

namespace MiniRTLS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AssetsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/asset
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Asset>>> GetAssets()
    {
        return await _context.Assets.Include(a => a.Telemetries).ToListAsync();
    }

    // GET: api/asset/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Asset>> GetAsset(int id)
    {
        var asset = await _context.Assets.Include(a => a.Telemetries)
                                         .FirstOrDefaultAsync(a => a.Id == id);

        if (asset == null) return NotFound();

        return asset;
    }

    // GET: api/assets/5/lastTelemetry
    [HttpGet("{id}/lastTelemetry")]
    public async Task<ActionResult<Telemetry>> GetLastTelemetry(int id)
    {
        var telemetry = await _context.Telemetries
                                    .Where(t => t.AssetId == id)
                                    .OrderByDescending(t => t.Timestamp)
                                    .FirstOrDefaultAsync();

        if (telemetry == null)
        {
            return NotFound($"Asset ID {id} için hiç telemetry verisi yok.");
        }

        return telemetry;
    }

    // GET: api/assets/active
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Asset>>> GetActiveAssets()
    {
        var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);

        var activeAssetIds = await _context.Telemetries
            .Where(t => t.Timestamp >= oneMinuteAgo)
            .Select(t => t.AssetId)
            .Distinct()
            .ToListAsync();

        var activeAssets = await _context.Assets
            .Where(a => activeAssetIds.Contains(a.Id))
            .ToListAsync();

        return activeAssets;
    }

    // POST: api/assets
    [HttpPost]
    public async Task<ActionResult<Asset>> CreateAsset(Asset asset)
    {
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
    }

    // PUT: api/assets/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsset(int id, Asset updatedAsset)
    {
        if (id != updatedAsset.Id) return BadRequest();

        _context.Entry(updatedAsset).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Assets.Any(a => a.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }
    
    [HttpGet("withLastTelemetry")]
    public async Task<ActionResult<IEnumerable<object>>> GetAssetsWithLastTelemetry()
    {
        var assetsWithLastTelemetry = await _context.Assets
            .Select(asset => new
            {
                asset.Id,
                asset.Name,
                asset.Type,
                asset.Status,
                LastTelemetry = asset.Telemetries
                    .OrderByDescending(t => t.Timestamp)
                    .Select(t => new { t.X, t.Y, t.Temperature, t.Timestamp })
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(assetsWithLastTelemetry);
    }

}
