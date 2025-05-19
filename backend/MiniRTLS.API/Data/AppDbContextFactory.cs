using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using System.IO;

namespace MiniRTLS.API.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlite(connectionString); // <--- burası değişti

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
