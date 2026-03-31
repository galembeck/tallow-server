using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFreteCheckoutResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("purchase")]
    public SuperFretePurchase? Purchase { get; set; }
}

public class SuperFretePurchase
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("orders")]
    public List<SuperFretePurchaseOrder> Orders { get; set; } = new();
}

public class SuperFretePurchaseOrder
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    [JsonPropertyName("service_id")]
    public int ServiceId { get; set; }

    [JsonPropertyName("tracking")]
    public string? Tracking { get; set; }

    [JsonPropertyName("print")]
    public SuperFretePrintInfo? Print { get; set; }
}

public class SuperFretePrintInfo
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
