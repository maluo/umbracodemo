using FundsApi.Models.ExportModels;

namespace FundsApi.Services;

/// <summary>
/// Generic Excel export service for exporting any list of objects to Excel
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// Export a list of objects to Excel
    /// </summary>
    /// <typeparam name="T">Type of objects to export</typeparam>
    /// <param name="data">List of objects to export</param>
    /// <param name="columns">Column definitions</param>
    /// <param name="options">Export options (optional)</param>
    /// <returns>Excel file as byte array</returns>
    byte[] ExportToExcel<T>(
        IEnumerable<T> data,
        IList<ExcelColumnDefinition> columns,
        ExcelExportOptions? options = null);
}
