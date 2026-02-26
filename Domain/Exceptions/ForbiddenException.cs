namespace Domain.Exceptions;

[Serializable]
public class ForbiddenException : System.Exception
{
    public ForbiddenException() : base()
    {
    }
}
