using System.Linq.Expressions;
using AutoMapper;
using HotelListing.Data.DTOs;
using HotelListing.Data.Model;
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
    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;

    public AccountController(IMapper mapper, ILogger<AccountController> logger, UserManager<ApiUser> userManager)
    {
        _mapper = mapper;
        _logger = logger;
        // _signInManager = signInManager;
        _userManager = userManager;
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

        try
        {
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

            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(Register)} method.");
            return Problem($"Something went wrong in the {nameof(Register)} method.", statusCode: 500);
        }
    }
    
    // [HttpPost]
    // [Route("login")]
    // public async Task<IActionResult> Login([FromBody] LoginUserDTO userDto)
    // {
    //     _logger.LogInformation($"Login attempt for {userDto.Email} ");
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }
    //
    //     try
    //     {
    //         SignInResult? result = await _signInManager.PasswordSignInAsync(userDto.Email, userDto.Password,
    //             isPersistent: false, lockoutOnFailure: false);
    //
    //         if (!result.Succeeded)
    //         {
    //             return Unauthorized(userDto);
    //         }
    //
    //         return Accepted();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, $"Something went wrong in the {nameof(Login)} method.");
    //         return Problem($"Something went wrong in the {nameof(Login)} method.", statusCode: 500);
    //     }
    // }
}