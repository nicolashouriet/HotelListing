using AutoMapper;
using HotelListing.Data;
using HotelListing.Data.DTOs;
using HotelListing.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountryController> _logger;
    private readonly IMapper _mapper;

    public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    public async Task<IActionResult> GetCountries()
    {
        try
        {
            var countries = await _unitOfWork.Countries.GetAll();
            var result = _mapper.Map<List<CountryDTO>>(countries);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountries)} method: ");
            return StatusCode(500, "Internal servor error.");
        }
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    public async Task<IActionResult> GetCountry(int id)
    {
        try
        {
            var country = await _unitOfWork.Countries.Get(c => c.Id == id, new List<string>() { "Hotels" });
            var result = _mapper.Map<CountryDTO>(country);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountry)} method: ");
            return StatusCode(500, "Internal servor error.");
        }
    }
    
    [Authorize(Roles="Admin")]
    [HttpPost]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status201Created))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError($"Invalid POST attempt in {nameof(CreateCountry)}");
            return BadRequest(ModelState);
        }

        try
        {
            Country country = _mapper.Map<Country>(countryDto);
            await _unitOfWork.Countries.Insert(country);
            await _unitOfWork.Save();

            return CreatedAtRoute("CreateCountry", new {id = country.Id}, country);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(CreateCountry)} method: ");
            return StatusCode(500, "Internal servor error.");
        }
    }
    
    [Authorize]
    [HttpPut("{id:int}")]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDto)
    {
        if (!ModelState.IsValid || id < 1)
        {
            _logger.LogError($"Invalid PUT attempt in {nameof(UpdateCountry)}");
            return BadRequest(ModelState);
        }

        try
        {
            Country country = await _unitOfWork.Countries.Get(q => q.Id == id);
            if (country == null)
            {
                _logger.LogError($"Invalid PUT attempt in {nameof(UpdateCountry)}");
                return BadRequest("The provided id does not match a DB country.");
            }

            _mapper.Map(countryDto, country);
            // since AsNoTracking() has been called on Get()
            _unitOfWork.Countries.Update(country);
            await _unitOfWork.Save();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(UpdateCountry)} method: ");
            return StatusCode(500, "Internal servor error.");
        }
    }
    
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        try
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
                return BadRequest(ModelState);
            }
            
            Country country = await _unitOfWork.Countries.Get(q => q.Id == id);
            if (country == null)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
                return BadRequest("The provided id does not match a DB hotel.");
            }

            await _unitOfWork.Countries.Delete(id);
            await _unitOfWork.Save();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(DeleteCountry)} method: ");
            return StatusCode(500, "Internal servor error.");
        }
    }
}