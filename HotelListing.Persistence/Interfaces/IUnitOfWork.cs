using HotelListing.Data;

namespace HotelListing.Persistence;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Country> Countries { get; }
    
    IGenericRepository<Hotel> Hotels { get; }

    Task Save();
}