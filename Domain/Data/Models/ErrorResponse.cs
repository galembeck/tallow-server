using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public record ErrorResponse
{
    /// <summary>
    /// Código de status HTTP.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    /// <summary>
    /// Título do erro (opcional).
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Mensagem de erro geral.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// Detalhes do erro, incluindo o campo que causou o erro e a mensagem específica.
    /// </summary>
    [JsonPropertyName("error")]
    public ErrorDetail Error { get; set; }

    /// <summary>
    /// Converte o objeto ErrorResponse para JSON.
    /// </summary>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}

public record ErrorDetail
{
    /// <summary>
    /// O campo que causou o erro.
    /// </summary>
    [JsonPropertyName("property")]
    public string Property { get; set; } = string.Empty;

    /// <summary>
    /// A mensagem de erro específica para o campo.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
