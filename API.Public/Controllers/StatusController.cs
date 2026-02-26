using API.Public.Controllers._Base;
using Domain.Constants;
using Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class StatusController : _BaseController
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Api = "Tallow - Public API",
            status = "OK",
            serverTime = StringUtil.GetDateFormated(DateFormatConstants.ISO_8601),
            uptime = (DateTimeOffset.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()).ToString(@"dd\.hh\:mm\:ss"),
            environment = Constant.Settings.Environment,
            version = Constant.Settings.Version,
        });
    }
}
