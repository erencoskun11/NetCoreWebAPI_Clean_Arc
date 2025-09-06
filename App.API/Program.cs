using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using App.Repositories; // AppDbContext'in oldu�u namespace - gerekirse de�i�tir
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// PostgreSQL / EF Core setup
// appsettings.json i�inde "DefaultConnection" tan�ml� olmal�.
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
// Bu k�s�m, uygulama ba�larken migration'lar� veritaban�na uygular.
// Hata durumunda log'lay�p uygulamay� devam ettirir.
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
        // production ortam�nda buray� daha sofistike logging ile de�i�tir
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
