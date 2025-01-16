namespace Core.Models;

public class PagedList<T>(List<T> items, int count, int pageNumber, int pageSize)
{
    public List<T> Items { get; set; } = items;
    public int TotalPages { get; set; } = (int)Math.Ceiling(count / (double)pageSize);
    public int CurrentPage { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int TotalCount { get; set; } = count;
    
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    
    public static PagedList<T> CreatePagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}