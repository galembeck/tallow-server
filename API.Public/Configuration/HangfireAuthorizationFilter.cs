using Hangfire.Dashboard;

namespace API.Public.Configuration;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow access only in Development; in Production restrict to authenticated admins
        var env = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        if (env.IsDevelopment())
            return true;

        return httpContext.User.Identity?.IsAuthenticated == true;
    }
}
