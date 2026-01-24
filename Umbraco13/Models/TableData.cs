using Microsoft.AspNetCore.Html;

namespace Umbraco13.Models;

/// <summary>
/// Represents the type of a table cell
/// </summary>
public enum TableCellType
{
    /// <summary>Header cell (th)</summary>
    Th,
    /// <summary>Data cell (td)</summary>
    Td
}

/// <summary>
/// Represents a single cell in a table
/// </summary>
public class TableCell
{
    /// <summary>
    /// Gets the type of the cell (th or td)
    /// </summary>
    public TableCellType Type { get; }

    /// <summary>
    /// Gets the HTML content of the cell
    /// </summary>
    public IHtmlContent Value { get; }

    /// <summary>
    /// Gets the CSS class for the cell
    /// </summary>
    public string? CssClass { get; }

    /// <summary>
    /// Gets the scope attribute for header cells (col, row, or null)
    /// </summary>
    public string? Scope { get; }

    public TableCell(TableCellType type, string value, string? cssClass = null, string? scope = null)
    {
        Type = type;
        Value = new HtmlString(value);
        CssClass = cssClass;
        Scope = scope;
    }
}

/// <summary>
/// Represents a table data model with cells organized in rows
/// </summary>
public class TableData
{
    /// <summary>
    /// Gets the CSS class for the table
    /// </summary>
    public string TableClass { get; set; } = "table table-striped";

    /// <summary>
    /// Gets the cells organized as a 2D array (rows, then columns)
    /// </summary>
    public IReadOnlyList<IReadOnlyList<TableCell>> Cells { get; }

    public TableData(IReadOnlyList<IReadOnlyList<TableCell>> cells)
    {
        Cells = cells;
    }

    /// <summary>
    /// Creates an empty table
    /// </summary>
    public static TableData Empty() => new(Array.Empty<IReadOnlyList<TableCell>>());
}
