using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFreteCartRequest
{
    [JsonPropertyName("from")]
    public SuperFreteCartSenderAddress From { get; set; }

    [JsonPropertyName("to")]
    public SuperFreteCartRecipientAddress To { get; set; }

    [JsonPropertyName("service")]
    public int Service { get; set; }

    [JsonPropertyName("volumes")]
    public SuperFreteCartVolumes Volumes { get; set; }

    [JsonPropertyName("products")]
    public List<SuperFreteCartProduct>? Products { get; set; }

    [JsonPropertyName("options")]
    public SuperFreteCartShippingOptions? Options { get; set; }

    [JsonPropertyName("platform")]
    public string? Platform { get; set; } = "Tallow";
}

public class SuperFreteCartSenderAddress
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; }

    [JsonPropertyName("complement")]
    public string? Complement { get; set; }

    [JsonPropertyName("district")]
    public string District { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("state_abbr")]
    public string StateAbbr { get; set; }

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; }

    [JsonPropertyName("document")]
    public string? Document { get; set; }
}

public class SuperFreteCartRecipientAddress
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; }

    [JsonPropertyName("complement")]
    public string? Complement { get; set; }

    [JsonPropertyName("district")]
    public string District { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("state_abbr")]
    public string StateAbbr { get; set; }

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; }

    [JsonPropertyName("document")]
    public string Document { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

public class SuperFreteCartVolumes
{
    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("width")]
    public float Width { get; set; }

    [JsonPropertyName("length")]
    public float Length { get; set; }

    [JsonPropertyName("weight")]
    public float Weight { get; set; }
}

public class SuperFreteCartProduct
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitary_value")]
    public decimal UnitaryValue { get; set; }
}

public class SuperFreteCartShippingOptions
{
    [JsonPropertyName("insurance_value")]
    public decimal? InsuranceValue { get; set; }

    [JsonPropertyName("receipt")]
    public bool Receipt { get; set; }

    [JsonPropertyName("own_hand")]
    public bool OwnHand { get; set; }

    [JsonPropertyName("non_commercial")]
    public bool NonCommercial { get; set; }

    [JsonPropertyName("invoice")]
    public SuperFreteCartInvoice? Invoice { get; set; }
}

public class SuperFreteCartInvoice
{
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }
}
