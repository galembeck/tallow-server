using Domain.Enumerators;
using Domain.SearchParameters._Base;

namespace Domain.SearchParameters;

public class ProductSearchParameter : BaseSearchParameter
{
    public ProductCategory? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
    public bool? InStock { get; set; }

    public ProductSearchParameter(BaseSearchParameter searchParameter = null) : base(searchParameter) { }
}
