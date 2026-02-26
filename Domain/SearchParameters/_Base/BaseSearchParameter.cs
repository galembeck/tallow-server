namespace Domain.SearchParameters._Base;

public class BaseSearchParameter
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string OrderBy { get; set; }
    public Boolean IsDesc { get; set; }

    public BaseSearchParameter(BaseSearchParameter? o = null)
    {
        OrderBy = "CREATEDAT";
        Page = 1;
        PageSize = 10;
        IsDesc = true;

        if (o != null)
        {
            Page = o.Page;
            PageSize = o.PageSize;
            OrderBy = o.OrderBy;
            IsDesc = o.IsDesc;
        }
    }
}
