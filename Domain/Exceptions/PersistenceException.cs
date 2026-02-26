using Domain.Enumerators;
using Domain.Utils;

namespace Domain.Exceptions;

[Serializable]
public class PersistenceException : ApplicationException
{
    public PersistenceException(Exception innerExc) : base(innerExc.GetType().Name, innerExc)
    {
    }

    public PersistenceException(string innerExc) : base(innerExc)
    {
    }

    public PersistenceException(PersistenceErrorMessage msg) : base(msg.GetDescription())
    {
    }
}
