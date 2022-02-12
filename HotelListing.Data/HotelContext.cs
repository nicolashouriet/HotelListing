using System.Collections.Immutable;
using System.Configuration;
using HotelListing.Data.Configurations.Entities;
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

        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new CountryConfiguration());
        modelBuilder.ApplyConfiguration(new HotelConfiguration());
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