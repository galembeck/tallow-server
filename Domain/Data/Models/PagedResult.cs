namespace Domain.Data.Models;

public abstract class PagedResultBase
{
    public int PageCount { get; set; }
    public int RowCount { get; set; }
}

public class PagedResult<T> : PagedResultBase where T : class
{
    public List<T> Rows { get; set; }

    public PagedResult()
    {
        Rows = new List<T>();
    }

    public PagedResult(List<T> rows, int rowCount, int pageCount)
    {
        Rows = new List<T>();
        Rows.AddRange(rows);
        RowCount = rowCount;
        PageCount = pageCount;
    }
}
