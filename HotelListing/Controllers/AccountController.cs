using System.Linq.Expressions;
using AutoMapper;
using HotelListing.Data.DTOs;
using HotelListing.Data.Model;
using HotelListing.Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace HotelListing.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : Controller
{
    private readonly UserManager<ApiUser> _userManager;

    // not needed thanks to the tokens system
    //private readonly SignInManager<ApiUser> _signInManager;
    private readonly IAuthManager _authManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;

    public AccountController(IMapper mapper, ILogger<AccountController> logger, UserManager<ApiUser> userManager,
        IAuthManager authManager)
    {
        _mapper = mapper;
        _logger = logger;
        // _signInManager = signInManager;
        _userManager = userManager;
        _authManager = authManager;
    }

    [HttpPost]
    [Route("register")]
    // Name of the methods is irrelevant for the route resolution, only the verb and other data specified in attributes
    // are relevant
    public async Task<IActionResult> Register([FromBody] UserDTO userDto)
    {
        _logger.LogInformation($"Registration attempt for {userDto.Email} ");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ApiUser? user = _mapper.Map<ApiUser>(userDto);
        user.UserName = userDto.Email;
        IdentityResult? result = await _userManager.CreateAsync(user, userDto.Password);

        if (!result.Succeeded)
        {
            string errorMessage = "User registration attempt failed!";

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
                errorMessage += "\r" + error.Description;
            }

            return BadRequest(errorMessage);
        }

        await _userManager.AddToRolesAsync(user, userDto.Roles);
        return Accepted();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO userDto)
    {
        _logger.LogInformation($"Login attempt for {userDto.Email} ");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _authManager.ValidateUser(userDto))
        {
            return Unauthorized();
        }

        return Accepted(new {Token = await _authManager.CreateToken()});
    }
}