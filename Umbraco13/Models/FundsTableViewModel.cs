namespace Umbraco13.Models;

public class FundsTableViewModel
{
    public TableData? TableData { get; set; }
    public string? PdfDownloadToken { get; set; }
    public string? CsvDownloadToken { get; set; }
    public string? ExcelDownloadToken { get; set; }
    public string? ExcelEpPlusDownloadToken { get; set; }
}
