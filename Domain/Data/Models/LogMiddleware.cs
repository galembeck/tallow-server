namespace Domain.Data.Models;

public class LogMiddleware
{
    public string ExceptionType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string RemoteIpAddress { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ExceptionMessage { get; set; } = string.Empty;
    public string ExceptionInnerMessage { get; set; } = string.Empty;
    public string ExceptionStackTrace { get; set; } = string.Empty;
    public string ExceptionSource { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
