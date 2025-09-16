using App.Repositories;
using App.Repositories.Extensions;
using App.Repositories.Interceptors;
using App.Services;
using App.Services.Categories;
using App.Services.Extensions;
using App.Services.Products;
using App.Services.Products.Create;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 🔹 Register AuditDbContextInterceptor
builder.Services.AddScoped<AuditDbContextInterceptor>();

// 🔹 DbContext (debug logging, sensitive data logging ve interceptor eklendi)
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
    options.UseNpgsql(connectionString)
           .EnableSensitiveDataLogging() // sadece dev ortamı için
           .LogTo(Console.WriteLine, LogLevel.Information)
           .AddInterceptors(serviceProvider.GetRequiredService<AuditDbContextInterceptor>())
);

// 🔹 Repository ve Service katmanları
builder.Services.AddRepositories(builder.Configuration)
                .AddServices(builder.Configuration);

// 🔹 AutoMapper profilleri
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CategoryProfileMapping>();
    cfg.AddProfile<ProductsMappingProfile>();
});

// 🔹 FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

// 🔹 Controllers
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// 🔹 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 🔹 Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// 🔹 Development/Production exception handling
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

// 🔹 AutoMapper ve DB migration kontrolü
using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;

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

    var db = provider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("Connected DB (app): " + db.Database.GetDbConnection().Database);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Could not read DB connection info: " + ex);
    }

    db.Database.Migrate();
}

// 🔹 Middleware pipeline
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
