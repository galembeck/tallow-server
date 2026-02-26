using API.Public.Controllers._Base;
using Domain.Enumerators;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Public.Filters;

public class AuthorizeAttribute : ActionFilterAttribute
{
    private readonly ProfileType[] AuthorizedProfiles;

    public AuthorizeAttribute(params ProfileType[] profileList)
    {
        AuthorizedProfiles = profileList;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (_BaseController.Authenticated.User is not null)
        {
            if (!AuthorizedProfiles.Any(x => x == _BaseController.Authenticated.User.ProfileType))
                throw new ForbiddenException();
        }
        else
            throw new ForbiddenException();
    }
}
