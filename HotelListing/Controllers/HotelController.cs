using AutoMapper;
using HotelListing.Data.DTOs;
using HotelListing.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HotelController> _logger;
    private readonly IMapper _mapper;

    public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    public async Task<IActionResult> GetHotels()
    {
        try
        {
            var hotels = await _unitOfWork.Hotels.GetAll();
            var result = _mapper.Map<List<HotelDTO>>(hotels);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(GetHotels)} method: ");
            return StatusCode(500, "Internal servor error.");
        }
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    [Authorize]
    public async Task<IActionResult> GetHotel(int id)
    {
        try
        {
            var hotel = await _unitOfWork.Hotels.Get(c => c.Id == id, new List<string>() { "Country" });
            var result = _mapper.Map<HotelDTO>(hotel);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(GetHotel)} method: ");
            return StatusCode(500, "Internal servor error.");
        }
    }
}