using System.Text;
using AspNetCoreRateLimit;
using HotelListing.Data.DTOs;
using HotelListing.Data.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace HotelListing.Data;

public static class ServiceExtensions
{
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        IdentityBuilder? builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);
        builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
        builder.AddEntityFrameworkStores<HotelContext>().AddTokenProvider<EmailTokenProvider<ApiUser>>("emailProvider");
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection? jwtSettings = configuration.GetSection("JwtSettings");
        // for a real api, take the issuer and key from environment variables
        // the key must be of size > Int.32
        string key = "DEVPOPONANADEVPOPONANADEVPOPONANADEVPOPONANADEVPOPONANA";

        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "HotelListing",
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
    }

    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(error =>
        {
            error.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    Log.Error($"Something went wrong in the {contextFeature.Error}");
                    await context.Response.WriteAsync(new Error
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Internal server error, please try again later."
                    }.ToString());
                }
            });
        });
    }
    
    // package Microsoft.AspNetCore.Mvc.Versioning not available for dotnet 6
    // public static void ConfigureVersioning(this IServiceCollection serviceCollection)
    // {
    //     serviceCollection.AddApiVersioning()
    // }
    public static void ConfigureRateLimiting(this IServiceCollection services)
    {
        List<RateLimitRule> rateLimitRules = new List<RateLimitRule>()
        {
            new RateLimitRule()
            {
                Endpoint = "*",
                Limit = 1,
                Period = "5s"
            }
        };
        services.Configure<IpRateLimitOptions>(opt =>
        {
            opt.GeneralRules = rateLimitRules;
        });
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }
}