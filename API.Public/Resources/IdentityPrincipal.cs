using Domain.Data.Entities;
using System.Security.Principal;

namespace API.Public.Resources;

public class IdentityPrincipal : IPrincipal
{
    public User User { get; }
    public IIdentity Identity { get; }

    public IdentityPrincipal(IIdentity identity, User user)
    {
        Identity = identity;
        User = user;
    }

    public bool IsInRole(string role) => true;
}
