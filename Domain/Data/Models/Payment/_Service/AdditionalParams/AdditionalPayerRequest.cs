using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class AdditionalPayerRequest
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("phone")]
    public PhoneRequest? Phone { get; set; }

    [JsonPropertyName("address")]
    public AddressRequest? Address { get; set; }

    [JsonPropertyName("registration_date")]
    public DateTimeOffset? RegistrationDate { get; set; }

    [JsonPropertyName("is_first_purchase_online")]
    public bool? IsFirstPurchaseOnline { get; set; }

    [JsonPropertyName("last_purchase")]
    public DateTimeOffset? LastPurchase { get; set; }
}
