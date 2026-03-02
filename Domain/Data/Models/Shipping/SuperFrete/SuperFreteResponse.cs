using Domain.Utils;
using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFreteResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("price")]
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string Price { get; set; }

    [JsonPropertyName("discount")]
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string Discount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("delivery_time")]
    public int DeliveryTime { get; set; }

    [JsonPropertyName("delivery_range")]
    public DeliveryRange DeliveryRange { get; set; }

    [JsonPropertyName("custom_delivery_time")]
    public int CustomDeliveryTime { get; set; }

    [JsonPropertyName("custom_delivery_range")]
    public DeliveryRange CustomDeliveryRange { get; set; }

    [JsonPropertyName("packages")]
    public List<PackageResponse> Packages { get; set; }

    [JsonPropertyName("additional_services")]
    public AdditionalServices AdditionalServices { get; set; }

    [JsonPropertyName("company")]
    public CompanyInfo Company { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }
}

public class DeliveryRange
{
    [JsonPropertyName("min")]
    public int Min { get; set; }

    [JsonPropertyName("max")]
    public int Max { get; set; }
}

public class PackageResponse
{
    [JsonPropertyName("price")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float Price { get; set; }

    [JsonPropertyName("discount")]
    public string Discount { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; }

    [JsonPropertyName("weight")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float Weight { get; set; }

    [JsonPropertyName("insurance_value")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float InsuranceValue { get; set; }

    [JsonPropertyName("dimensions")]
    public Dimensions Dimensions { get; set; }
}

public class Dimensions
{
    [JsonPropertyName("height")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float Height { get; set; }

    [JsonPropertyName("width")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float Width { get; set; }

    [JsonPropertyName("length")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float Length { get; set; }
}

public class AdditionalServices
{
    [JsonPropertyName("receipt")]
    public bool Receipt { get; set; }

    [JsonPropertyName("own_hand")]
    public bool OwnHand { get; set; }

    [JsonPropertyName("collect")]
    public bool Collect { get; set; }
}

public class CompanyInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("picture")]
    public string Picture { get; set; }
}