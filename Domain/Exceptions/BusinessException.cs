using Domain.Enumerators;
using Domain.Utils;

namespace Domain.Exceptions;

[Serializable]
public class BusinessException : ApplicationException
{
    public IDictionary<String, String> ErrorList { get; }

    public BusinessException(System.Exception ex, IDictionary<string, string> errorList = null) : base(ex.GetType().Name, ex)
    {
        ErrorList = errorList;
    }

    public BusinessException(string msg, IDictionary<string, string> errorList = null) : base(msg)
    {
        ErrorList = errorList;
    }

    public BusinessException(BusinessErrorMessage msg, IDictionary<string, string> errorList = null) : base(msg.GetDescription())
    {
        ErrorList = errorList;
    }
}