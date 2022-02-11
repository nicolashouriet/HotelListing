using System.Collections.Immutable;
using System.Configuration;
using HotelListing.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace HotelListing.Data;

public class HotelContext : IdentityDbContext<ApiUser>
{
    public HotelContext(DbContextOptions options) : base(options)
    {

    }

    public HotelContext()
    {
        
    }
    
    public DbSet<Country> Countries { get; set; }
        
    public DbSet<Hotel> Hotels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // for IdentityDbContext
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().HasData(
            new Country()
            {
                Id = 1,
                Name = "Switzerland",
                ShortName = "CH"
            },
            new Country()
            {
                Id = 2,
                Name = "Germany",
                ShortName = "DE"
            },
            new Country()
            {
                Id = 3,
                Name = "France",
                ShortName = "FR"
            });

        modelBuilder.Entity<Hotel>().HasData(
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // only configure if not done via the ctor
        if (optionsBuilder.IsConfigured == false)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.IntegratedSecurity = true;
            builder.DataSource = @"(LocalDb)\MSSQLLocalDB";
            builder.InitialCatalog = "HotelListing";
            string connectionString = builder.ToString();

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}