using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Data.Configurations.Entities;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasData(
            new Hotel()
            {
                Id = 1,
                Address = "2 rue des acacias",
                CountryId = 1,
                Name = "Hotel des acacias",
                Rating = 4.0
            },
            new Hotel()
            {
                Id = 2,
                Address = "24 Berlin Strasse",
                CountryId = 2,
                Name = "Bahnhofhotel",
                Rating = 3.2
            },
            new Hotel()
            {
                Id = 3,
                Address = "15 rue des Champs Elysees",
                CountryId = 3,
                Name = "Hotel des Champs",
                Rating = 3.7
            });
    }
}