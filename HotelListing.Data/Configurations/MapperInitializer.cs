using AutoMapper;
using HotelListing.Data.DTOs;
using HotelListing.Data.Model;

namespace HotelListing.Data.Configurations;

public class MapperInitializer : Profile
{
    public MapperInitializer()
    {
        CreateMap<Country, CountryDTO>().ReverseMap();
        CreateMap<Country, CreateCountryDTO>().ReverseMap();
        CreateMap<Hotel, HotelDTO>().ReverseMap();
        CreateMap<Hotel, CreateHotelDTO>().ReverseMap();
        CreateMap<ApiUser, UserDTO>().ReverseMap();
    }
}