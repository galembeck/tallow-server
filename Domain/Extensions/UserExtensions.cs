using Domain.Data.Entities;

namespace Domain.Extensions;

public static class UserExtensions
{
    public static User BaseExample(this User user)
    {
        if (user.Email != null)
        {
            user.Email = user.Email.Trim();
        }

        return user;
    }
}
