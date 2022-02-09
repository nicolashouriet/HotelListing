using HotelListing.Data;

namespace HotelListing.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly HotelContext _context;
    private IGenericRepository<Country> _countries;
    private IGenericRepository<Hotel> _hotels;

    public UnitOfWork(HotelContext context)
    {
        _context = context;
    }

    public IGenericRepository<Country> Countries => _countries ??= new GenericRepository<Country>(_context);
    public IGenericRepository<Hotel> Hotels => _hotels ??= new GenericRepository<Hotel>(_context);
    
    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}