using Domain.SearchParameters._Base;

namespace Domain.SearchParameters.UserHstoric;

public class UserHistoricSearchParameter : BaseSearchParameter
{
    public string Document { get; set; }

    public UserHistoricSearchParameter() : base() { }

    public UserHistoricSearchParameter(BaseSearchParameter? searchParameter = null) : base(searchParameter)
    {
    }

}
