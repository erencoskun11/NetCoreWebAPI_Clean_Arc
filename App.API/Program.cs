using App.Repositories;
using App.Repositories.Categories;
using App.Repositories.Extensions;
using App.Repositories.Interceptors;
using App.Repositories.Products;
using App.Services;
using App.Services.Categories;
using App.Services.Extensions;
using App.Services.Products;
using App.Services.Products.Create;
using App.Services.Filters;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// Configuration & Connection
// ---------------------------
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

// ---------------------------
// Register Cross-cutting services
// ---------------------------
builder.Services.AddScoped<AuditDbContextInterceptor>();

// If your RepositoryExtensions registers open-generic IGenericRepository<,> then no need to register here.
// Call your extension to register repos and services (it should handle concrete repos too).
builder.Services.AddRepositories(configuration);
builder.Services.AddServices(configuration);

// ---------------------------
// DbContext
// ---------------------------
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(connectionString)
           .AddInterceptors(serviceProvider.GetRequiredService<AuditDbContextInterceptor>());

    // Sensitive data logging and console logging only in Development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging()
               .LogTo(Console.WriteLine, LogLevel.Information);
    }
});


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CategoryProfileMapping>();
    cfg.AddProfile<ProductsMappingProfile>();
});


builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();


builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// ---------------------------
// CORS
// ---------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ---------------------------
// Swagger / OpenAPI
// ---------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// ---------------------------
// Build app
// ---------------------------
var app = builder.Build();

// ---------------------------
// Error handling & Swagger
// ---------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}
else
{
    app.UseExceptionHandler(appError =>
    {
        appError.Run(async context =>
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error" });
        });
    });
}

// ---------------------------
// AutoMapper validation & DB migration (only Dev)
// ---------------------------
using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;

    // Validate AutoMapper configuration
    try
    {
        var mapper = provider.GetRequiredService<IMapper>();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
        Console.WriteLine("✅ AutoMapper configuration valid.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ AutoMapper config error: " + ex);
        throw;
    }

    // DB connection check and migrate (DEV only)
    var db = provider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("Connected DB (app): " + db.Database.GetDbConnection().Database);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Could not read DB connection info: " + ex);
    }

    if (builder.Environment.IsDevelopment())
    {
        db.Database.Migrate();
    }
}

// ---------------------------
// Middleware pipeline
// ---------------------------
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
