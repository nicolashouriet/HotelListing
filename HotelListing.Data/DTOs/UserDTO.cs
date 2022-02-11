using System.ComponentModel.DataAnnotations;

namespace HotelListing.Data.DTOs;

// UI -> DTO -> DOMAIN -> DB
public class UserDTO : LoginUserDTO
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Required]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
}

public class LoginUserDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    [StringLength(15, ErrorMessage = "Your password is limited from {2} to {1} characters", MinimumLength = 7)]
    public string Password { get; set; }
}