using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace App.Repositories
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1️⃣ appsettings.json'u yükle
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Çalışma dizini
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2️⃣ Connection string'i al
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 3️⃣ DbContextOptions oluştur
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
