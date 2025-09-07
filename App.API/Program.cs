using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using App.Repositories;
using System;
using App.Repositories.Extensions;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var connectionStrings = builder.Configuration.GetSection("ConnectionStrings")
                                             .Get<ConnectionStringOptions>();

builder.Services.AddRepositories(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

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
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
