using Umbraco13.Models;

namespace Umbraco13.Helpers;

/// <summary>
/// Helper class to convert Fund data to TableData format
/// </summary>
public static class FundTableConverter
{
    /// <summary>
    /// Converts a collection of Funds to TableData format
    /// The table is rotated with funds as columns and attributes as rows
    /// </summary>
    /// <param name="funds">The collection of funds to convert</param>
    /// <returns>TableData structure suitable for rendering</returns>
    public static TableData ToTableData(IEnumerable<Fund> funds)
    {
        var fundList = funds.ToList();

        if (fundList.Count == 0)
        {
            return TableData.Empty();
        }

        // Create cells list (2D list representing rows, then columns)
        var cells = new List<List<TableCell>>();

        // Header row (fund names) - marked as <th> cells
        var headerRow = new List<TableCell>
        {
            new TableCell(TableCellType.Th, "Attribute", "text-start", "col")
        };

        for (int i = 0; i < fundList.Count; i++)
        {
            headerRow.Add(new TableCell(TableCellType.Th, fundList[i].FundName, "text-center", "col"));
        }
        cells.Add(headerRow);

        // Ticker Code row
        var tickerRow = new List<TableCell>
        {
            new TableCell(TableCellType.Td, "<strong>Ticker Code</strong>", "text-start")
        };
        for (int i = 0; i < fundList.Count; i++)
        {
            tickerRow.Add(new TableCell(TableCellType.Td, fundList[i].TickerCode, "text-center"));
        }
        cells.Add(tickerRow);

        // NAV Price row
        var navRow = new List<TableCell>
        {
            new TableCell(TableCellType.Td, "<strong>NAV Price</strong>", "text-start")
        };
        for (int i = 0; i < fundList.Count; i++)
        {
            navRow.Add(new TableCell(TableCellType.Td, fundList[i].NavPrice.ToString("C"), "text-center"));
        }
        cells.Add(navRow);

        // Market Price row
        var marketRow = new List<TableCell>
        {
            new TableCell(TableCellType.Td, "<strong>Market Price</strong>", "text-start")
        };
        for (int i = 0; i < fundList.Count; i++)
        {
            marketRow.Add(new TableCell(TableCellType.Td, fundList[i].MarketPrice.ToString("C"), "text-center"));
        }
        cells.Add(marketRow);

        // Hold in Trust row
        var trustRow = new List<TableCell>
        {
            new TableCell(TableCellType.Td, "<strong>Hold in Trust</strong>", "text-start")
        };
        for (int i = 0; i < fundList.Count; i++)
        {
            trustRow.Add(new TableCell(TableCellType.Td, fundList[i].HoldInTrust, "text-center"));
        }
        cells.Add(trustRow);

        return new TableData(cells);
    }
}
