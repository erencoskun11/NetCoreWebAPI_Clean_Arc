using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using App.Repositories; // AppDbContext'in olduðu namespace - gerekirse deðiþtir
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// PostgreSQL / EF Core setup
// appsettings.json içinde "DefaultConnection" tanýmlý olmalý.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Apply EF Core migrations at startup (automatically)
// Bu kýsým, uygulama baþlarken migration'larý veritabanýna uygular.
// Hata durumunda log'layýp uygulamayý devam ettirir.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("Database migrated successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error while applying migrations: " + ex);
        // production ortamýnda burayý daha sofistike logging ile deðiþtir
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
