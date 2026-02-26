using Domain.Enumerators;

namespace Domain.Exceptions;

[Serializable]
public class AuthenticationException : Exception
{
    public AuthenticationException(Exception innerExc) : base(innerExc.GetType().Name, innerExc)
    {
    }

    public AuthenticationException(string innerExc) : base(innerExc)
    {
    }

    public AuthenticationException(AuthenticationErrorMessage innerExc) : base(innerExc.ToString())
    {
    }
}
