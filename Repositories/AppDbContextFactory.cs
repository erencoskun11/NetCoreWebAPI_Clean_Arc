using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace App.Repositories
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Buraya migration çalıştırırken kullanılacak connection string'i yaz.
            // (Güvenlik için production'ta burada şifre saklama. Sadece migration için uygun.)
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=NetCoreWebAPI_DB;Username=guest;Password=guest");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
