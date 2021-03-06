using AutoMapper;
using HotelListing.Data;
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
        var hotels = await _unitOfWork.Hotels.GetAll();
        var result = _mapper.Map<List<HotelDTO>>(hotels);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    public async Task<IActionResult> GetHotel(int id)
    {
        var hotel = await _unitOfWork.Hotels.Get(c => c.Id == id, new List<string>() {"Country"});
        var result = _mapper.Map<HotelDTO>(hotel);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status201Created))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
            return BadRequest(ModelState);
        }

        Hotel hotel = _mapper.Map<Hotel>(hotelDto);
        await _unitOfWork.Hotels.Insert(hotel);
        await _unitOfWork.Save();

        return CreatedAtRoute("GetHotel", new {id = hotel.Id}, hotel);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    [ProducesResponseType((StatusCodes.Status500InternalServerError))]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDto)
    {
        if (!ModelState.IsValid || id < 1)
        {
            _logger.LogError($"Invalid PUT attempt in {nameof(UpdateHotel)}");
            return BadRequest(ModelState);
        }

        Hotel hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
        if (hotel == null)
        {
            _logger.LogError($"Invalid PUT attempt in {nameof(UpdateHotel)}");
            return BadRequest("The provided id does not match a DB hotel.");
        }

        _mapper.Map(hotelDto, hotel);
        // since AsNoTracking() has been called on Get()
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.Save();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        if (id < 1)
        {
            _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
            return BadRequest(ModelState);
        }

        Hotel hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
        if (hotel == null)
        {
            _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
            return BadRequest("The provided id does not match a DB hotel.");
        }

        await _unitOfWork.Hotels.Delete(id);
        await _unitOfWork.Save();

        return NoContent();
    }
}