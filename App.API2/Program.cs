using App.API2.ExceptionHandlers;
using App.API2.Filters;
using App.Application;
using App.Application.Contracts.Caching;
using App.Application.Extensions;
using App.Bus;
using App.Caching;
using App.Persistance;
using App.Persistance.Extensions;
using App.Services.ExceptionHandlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Web services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authorization
builder.Services.AddAuthorization(); // <-- eksik olan kýsým

// Filters & Exception Handlers
builder.Services.AddScoped(typeof(NotFoundFilter<,>));
builder.Services.AddExceptionHandler<CriticalExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Caching
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddMemoryCache();

// Application & Persistence
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

// Bus
builder.Services.AddRepositories(builder.Configuration)
                .AddApplicationServices(builder.Configuration)
                .AddBus(builder.Configuration);

var app = builder.Build();

// Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
