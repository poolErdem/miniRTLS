using Microsoft.EntityFrameworkCore;
using MiniRTLS.API.Models;

namespace MiniRTLS.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Telemetry> Telemetries => Set<Telemetry>();
}
