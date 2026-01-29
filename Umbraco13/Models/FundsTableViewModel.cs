namespace Umbraco13.Models;

public class FundsTableViewModel
{
    public TableData? TableData { get; set; }
    public List<Fund>? Funds { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortColumn { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public int TotalItems => Funds?.Count ?? 0;
    public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);
}
