using HotelListing.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers;

[ApiController]
// [Route("[controller]")]
public class CountryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public CountryController(IUnitOfWork unitOfWork, ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCountries()
    {
        try
        {
            var countries = _unitOfWork.Countries.GetAll();
            return Ok(countries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountries)} method: ");
            return StatusCode(500, "Internal servor error.");
        }
    }
}