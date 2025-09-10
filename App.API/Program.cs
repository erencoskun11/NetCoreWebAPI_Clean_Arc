using App.Repositories;
using App.Repositories.Extensions;
using App.Services;
using App.Services.Extensions;
using App.Services.Products;
using FluentValidation;
using FluentValidation.AspNetCore;
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

// Register validators from Services assembly
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

// Enable FluentValidation automatic validation (adds errors to ModelState)
builder.Services.AddFluentValidationAutoValidation();

// Controllers & add custom FluentValidationFilter to format errors as ServiceResult
builder.Services.AddControllers(options =>
{
    // Eðer FluentValidationFilter sýnýfýn ServiceResult.Fail(errors) döndürüyorsa
    options.Filters.Add<FluentValidationFilter>();
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// Keep default ModelState automatic 400 disabled because we use custom filter
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
