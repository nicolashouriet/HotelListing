using HotelListing.Data.DTOs;

namespace HotelListing.Data.Services;

public interface IAuthManager
{
    Task<bool> ValidateUser(LoginUserDTO loginUserDto);

    Task<string> CreateToken();
    
}