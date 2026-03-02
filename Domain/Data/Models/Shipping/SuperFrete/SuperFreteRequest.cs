using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFreteRequest
{
    [JsonPropertyName("from")]
    public FromAddress From { get; set; }

    [JsonPropertyName("to")]
    public ToAddress To { get; set; }

    [JsonPropertyName("services")]
    public string Services { get; set; }

    [JsonPropertyName("options")]
    public ShippingOptions Options { get; set; }

    [JsonPropertyName("products")]
    public List<ProductInfo>? Products { get; set; }

    [JsonPropertyName("package")]
    public PackageInfo? Package { get; set; }
}

public class FromAddress
{
    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; }
}

public class ToAddress
{
    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; }
}

public class ShippingOptions
{
    [JsonPropertyName("own_hand")]
    public bool OwnHand { get; set; } = false;

    [JsonPropertyName("receipt")]
    public bool Receipt { get; set; } = false;

    [JsonPropertyName("insurance_value")]
    public decimal InsuranceValue { get; set; } = 0;

    [JsonPropertyName("use_insurance_value")]
    public bool UseInsuranceValue { get; set; } = false;
}

public class ProductInfo
{
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("weight")]
    public float Weight { get; set; }

    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("width")]
    public float Width { get; set; }

    [JsonPropertyName("length")]
    public float Length { get; set; }
}

public class PackageInfo
{
    [JsonPropertyName("weight")]
    public float Weight { get; set; }

    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("width")]
    public float Width { get; set; }

    [JsonPropertyName("length")]
    public float Length { get; set; }
}