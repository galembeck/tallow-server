using Domain.Repository;
using Microsoft.AspNetCore.SignalR;

namespace API.Public.Hubs;

/// <summary>
/// SignalR hub for real-time admin notifications.
/// Connection requires a valid AccessToken cookie.
///
/// Client usage (JS):
///   const conn = new signalR.HubConnectionBuilder()
///     .withUrl("/hubs/notifications", { withCredentials: true })
///     .build();
///
///   conn.on("notification", (n) => console.log(n));
///   conn.start();
///
/// Filtering by category (optional):
///   await conn.invoke("Subscribe", "PAYMENT");   // receives only PAYMENT notifications
///   await conn.invoke("Subscribe", "ORDER");
///   await conn.invoke("Subscribe", "SHIPPING");
///   await conn.invoke("Unsubscribe", "PAYMENT");
/// </summary>
public class AdminNotificationHub : Hub
{
    private readonly IAccessTokenRepository _accessTokenRepository;

    public AdminNotificationHub(IAccessTokenRepository accessTokenRepository)
    {
        _accessTokenRepository = accessTokenRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var token = Context.GetHttpContext()?.Request.Cookies["AccessToken"];

        if (string.IsNullOrEmpty(token))
        {
            Context.Abort();
            return;
        }

        var accessToken = await _accessTokenRepository.GetAsync(token);

        if (accessToken is null || accessToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            Context.Abort();
            return;
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Join a category group to receive filtered notifications.
    /// Valid values: ORDER, PAYMENT, SHIPPING
    /// </summary>
    public async Task Subscribe(string category)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"category:{category.ToUpper()}");
    }

    /// <summary>
    /// Leave a category group.
    /// </summary>
    public async Task Unsubscribe(string category)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"category:{category.ToUpper()}");
    }
}
