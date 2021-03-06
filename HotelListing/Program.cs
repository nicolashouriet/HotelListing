using System.Configuration;
using System.Reflection;
using HotelListing.Data;
using HotelListing.Data.Configurations;
using HotelListing.Data.Services;
using HotelListing.Persistence;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(config =>
        config.CacheProfiles.Add("120sec", new CacheProfile()
        {
            Duration = 120
        }))
    .AddNewtonsoftJson(op =>
        op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimiting();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpCacheHeaders((expirationOpts) =>
    {
        expirationOpts.MaxAge = 120;
        expirationOpts.CacheLocation = CacheLocation.Private;
    },
    (validationOpt) => { validationOpt.MustRevalidate = true; });

builder.Services.AddCors(o => { o.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });
builder.Services.AddAutoMapper(typeof(MapperInitializer));

//TODO: optimize later as with this variant a new unit of work is instantiated for each request!
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);

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
    app.ConfigureExceptionHandler();
    app.UseCors("AllowAll");
    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRouting();
    app.UseResponseCaching();
    app.UseHttpCacheHeaders();
    app.MapControllers();

    // app.UseEndpoints(endpoints =>
    // {
    //     endpoints.MapControllerRoute(
    //         name: "default",
    //         pattern: "{controller=Country}/{action=Index}/{id?}"
    //     );
    //     endpoints.MapControllers();
    // });

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