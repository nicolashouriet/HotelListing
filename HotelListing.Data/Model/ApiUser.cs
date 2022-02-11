using Microsoft.AspNetCore.Identity;

namespace HotelListing.Data.Model;

public sealed class ApiUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}