using System.ComponentModel.DataAnnotations;

namespace HotelListing.Data.DTOs;

// UI -> DTO -> DOMAIN -> DB
public class CountryDTO : CreateCountryDTO
{
    public int Id { get; set; }
    
    public IList<HotelDTO> Hotels { get; set; }
}

// this DTO should be passed to the BL method in charge of creating a country.
// it therefore doesn't need to store an ID, since it isn't its responsibility
public class CreateCountryDTO
{
    [Required]
    [StringLength(maximumLength: 50, ErrorMessage = "Country name is too long!")]
    public string Name { get; set; }

    [Required]
    [StringLength(maximumLength: 2, ErrorMessage = "Short Country name name is too long!")]
    public string ShortName { get; set; }
}

public class UpdateCountryDTO : CreateCountryDTO
{
    public IList<CreateHotelDTO> Hotels { get; set; }
}