using System.Configuration;
using System.Reflection;
using HotelListing.Data;
using HotelListing.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HotelContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"));
});
builder.Logging.AddSerilog();

string logPath = Path.Combine(Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()), "HotelListingLogs",
    "log-.txt");
Log.Logger = new LoggerConfiguration().WriteTo
    .File(path: logPath, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

try
{
    Log.Information("Application configuration..");

    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Application is starting");
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}