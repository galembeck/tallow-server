using Domain.Enumerators;
using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class PayerRequest
{
    [JsonPropertyName("entity_type")]
    public string? EntityType { get; set; } = "individual";

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "customer";

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("identification")]
    public IdentificationRequest? Identification { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Required by Mercado Pago for Boleto payments.
    /// </summary>
    [JsonPropertyName("address")]
    public AddressRequest? Address { get; set; }
}
