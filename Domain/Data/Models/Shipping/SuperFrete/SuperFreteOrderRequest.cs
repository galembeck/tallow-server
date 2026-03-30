using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFreteOrderRequest
{
    [JsonPropertyName("service")]
    public int Service { get; set; }

    [JsonPropertyName("agency")]
    public int? Agency { get; set; }

    [JsonPropertyName("from")]
    public SuperFreteAddress From { get; set; }

    [JsonPropertyName("to")]
    public SuperFreteAddress To { get; set; }

    [JsonPropertyName("products")]
    public List<SuperFreteProduct> Products { get; set; }

    [JsonPropertyName("volumes")]
    public List<SuperFreteVolume> Volumes { get; set; }

    [JsonPropertyName("options")]
    public SuperFreteOrderOptions Options { get; set; }
}

public class SuperFreteAddress
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("document")]
    public string Document { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("complement")]
    public string? Complement { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; }

    [JsonPropertyName("district")]
    public string District { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("country_id")]
    public string CountryId { get; set; } = "BR";

    [JsonPropertyName("state_abbr")]
    public string StateAbbr { get; set; }

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }
}

public class SuperFreteProduct
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitary_value")]
    public decimal UnitaryValue { get; set; }

    [JsonPropertyName("weight")]
    public float Weight { get; set; }
}

public class SuperFreteVolume
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

public class SuperFreteOrderOptions
{
    [JsonPropertyName("insurance_value")]
    public decimal InsuranceValue { get; set; }

    [JsonPropertyName("receipt")]
    public bool Receipt { get; set; } = false;

    [JsonPropertyName("own_hand")]
    public bool OwnHand { get; set; } = false;

    [JsonPropertyName("collect")]
    public bool Collect { get; set; } = false;

    [JsonPropertyName("reverse")]
    public bool Reverse { get; set; } = false;

    [JsonPropertyName("non_commercial")]
    public bool NonCommercial { get; set; } = false;

    [JsonPropertyName("invoice")]
    public SuperFreteInvoice Invoice { get; set; } = new();
}

public class SuperFreteInvoice
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
}

public class SuperFreteCartPaymentRequest
{
    [JsonPropertyName("orders")]
    public List<string> Orders { get; set; }
}
