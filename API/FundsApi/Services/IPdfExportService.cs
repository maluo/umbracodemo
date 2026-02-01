using FundsApi.Models.ExportModels;

namespace FundsApi.Services;

/// <summary>
/// Interface for generic PDF export service
/// </summary>
public interface IPdfExportService
{
    /// <summary>
    /// Export a list of objects to PDF
    /// </summary>
    /// <typeparam name="T">Type of objects to export</typeparam>
    /// <param name="data">List of objects to export</param>
    /// <param name="columns">Column definitions</param>
    /// <param name="options">Export options (optional)</param>
    /// <returns>PDF file as byte array</returns>
    byte[] ExportToPdf<T>(
        IEnumerable<T> data,
        IList<PdfColumnDefinition> columns,
        PdfExportOptions? options = null);
}
