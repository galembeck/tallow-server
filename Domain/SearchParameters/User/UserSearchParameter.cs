using Domain.SearchParameters._Base;

namespace Domain.SearchParameters;

public class UserSearchParameter : BaseSearchParameter
{
    public string Name { get; set; }

    public UserSearchParameter(BaseSearchParameter? searchParameter = null) : base(searchParameter)
    {
    }
}