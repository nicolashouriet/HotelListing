using System.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelListing.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<HotelContext>
{
    public HotelContext CreateDbContext(string[] args)
    {
        Console.WriteLine("TEST");
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.IntegratedSecurity = true;
        builder.DataSource = @"(LocalDb)\MSSQLLocalDB";
        builder.InitialCatalog = "HotelListing";
        var optionsBuilder = new DbContextOptionsBuilder<HotelContext>();
        optionsBuilder.UseSqlServer(builder.ToString());

        return new HotelContext(optionsBuilder.Options);
    }
}