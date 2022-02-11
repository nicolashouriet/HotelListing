using HotelListing.Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HotelListing.Data;

public static class ServiceExtensions
{
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        IdentityBuilder? builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);
        builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
        builder.AddEntityFrameworkStores<HotelContext>().AddTokenProvider<EmailTokenProvider<ApiUser>>("emailProvider");
    }
}