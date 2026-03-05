using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class ReceiverAddressRequest
{
    [JsonPropertyName("zip_code")]
    public string? ZipCode { get; set; }

    [JsonPropertyName("state_name")]
    public string? StateName { get; set; }

    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    [JsonPropertyName("street_name")]
    public string? StreetName { get; set; }

    [JsonPropertyName("street_number")]
    public string? StreetNumber { get; set; }

    [JsonPropertyName("floor")]
    public string? Floor { get; set; }

    [JsonPropertyName("apartment")]
    public string? Apartment { get; set; }









    



    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }
}
