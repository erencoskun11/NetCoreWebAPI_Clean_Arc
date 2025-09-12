using App.Repositories;
using App.Repositories.Extensions;
using App.Services;
using App.Services.Extensions;
using App.Services.Products.Create;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext kaydý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repository & Service kayýtlarý
builder.Services.AddRepositories(builder.Configuration)
                .AddServices(builder.Configuration);

// Validator'larý register et (CreateProductRequestValidator'ýn bulunduðu assembly)
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

builder.Services.AddControllers();

// Biz manuel Response formatý kullanacaðýmýz için ASP.NET'in otomatik ModelState 400'ünü kapatýyoruz
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

app.UseExceptionHandler(x => {});

// Migration ve DB init
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
