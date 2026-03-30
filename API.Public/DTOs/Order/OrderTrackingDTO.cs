using Domain.Data.Models;

namespace API.Public.DTOs;

public class OrderTrackingDTO
{
    public string? TrackingCode { get; set; }
    public string? TrackingUrl { get; set; }
    public string? Status { get; set; }
    public List<OrderTrackingEventDTO> Events { get; set; } = new();

    public static OrderTrackingDTO FromSuperFrete(SuperFreteTrackingResponse sf) => new()
    {
        TrackingCode = sf.Tracking,
        TrackingUrl = sf.TrackingUrl,
        Status = sf.Status,
        Events = sf.Events?.Select(e => new OrderTrackingEventDTO
        {
            Status = e.Status,
            Message = e.Message,
            Location = e.Location,
            Date = e.Date
        }).ToList() ?? new()
    };
}

public class OrderTrackingEventDTO
{
    public string? Status { get; set; }
    public string? Message { get; set; }
    public string? Location { get; set; }
    public string? Date { get; set; }
}
