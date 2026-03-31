using Domain.Utils;
using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFreteOrderInfoResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("delivery")]
    public int Delivery { get; set; }

    [JsonPropertyName("delivery_min")]
    public int DeliveryMin { get; set; }

    [JsonPropertyName("delivery_max")]
    public int DeliveryMax { get; set; }

    [JsonPropertyName("discount")]
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string Discount { get; set; }

    [JsonPropertyName("height")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float Height { get; set; }

    [JsonPropertyName("width")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float Width { get; set; }

    [JsonPropertyName("length")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float Length { get; set; }

    [JsonPropertyName("weight")]
    [JsonConverter(typeof(FlexibleFloatConverter))]
    public float Weight { get; set; }

    [JsonPropertyName("price")]
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string Price { get; set; }

    [JsonPropertyName("tracking")]
    public string? Tracking { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("service_id")]
    public int ServiceId { get; set; }

    [JsonPropertyName("products")]
    public List<SuperFreteOrderInfoProduct>? Products { get; set; }

    [JsonPropertyName("insurance_value")]
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string? InsuranceValue { get; set; }

    [JsonPropertyName("generated_at")]
    public DateTime? GeneratedAt { get; set; }

    [JsonPropertyName("posted_at")]
    public DateTime? PostedAt { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

public class SuperFreteOrderInfoProduct
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("quantity")]
    public string Quantity { get; set; }

    [JsonPropertyName("unitary_value")]
    public string UnitaryValue { get; set; }
}
