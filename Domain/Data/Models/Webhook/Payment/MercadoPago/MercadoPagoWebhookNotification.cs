using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class MercadoPagoWebhookNotification
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("live_mode")]
    public bool LiveMode { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("date_created")]
    public DateTime DateCreated { get; set; }

    [JsonPropertyName("application_id")]
    public string ApplicationId { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("api_version")]
    public string ApiVersion { get; set; }

    [JsonPropertyName("action")]
    public string Action { get; set; }

    [JsonPropertyName("data")]
    public WebhookData Data { get; set; }
}

public class WebhookData
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}