using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelListing.Data.DTOs;
using HotelListing.Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HotelListing.Data.Services;

public class AuthManager : IAuthManager
{
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;
    private ApiUser _user;

    public AuthManager(IConfiguration configuration, UserManager<ApiUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<bool> ValidateUser(LoginUserDTO loginUserDto)
    {
        _user = await _userManager.FindByNameAsync(loginUserDto.Email);
        return (_user != null && await _userManager.CheckPasswordAsync(_user, loginUserDto.Password));
    }

    public async Task<string> CreateToken()
    {
        SigningCredentials signingCredentials = GetSigningCredentials();
        List<Claim> claims = await GetClaims();
        JwtSecurityToken tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "HotelListing",
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: signingCredentials
        );

        return token;
    }

    private async Task<List<Claim>> GetClaims()
    {
        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, _user.UserName));

        IList<string>? roles = await _userManager.GetRolesAsync(_user);

        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = "DEVPOPONANADEVPOPONANADEVPOPONANADEVPOPONANADEVPOPONANA";
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
}