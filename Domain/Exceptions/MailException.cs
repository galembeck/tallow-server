namespace Domain.Exceptions;

[Serializable]
public class MailException : ApplicationException
{
    public MailException(Exception innerExc) : base(innerExc.GetType().Name, innerExc)
    {
    }

    public MailException(string innerExc) : base(innerExc)
    {
    }
}