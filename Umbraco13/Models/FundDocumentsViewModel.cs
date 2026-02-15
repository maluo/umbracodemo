using System.Collections.Generic;
using Umbraco13.Models;

namespace Umbraco13.Models
{
    public class FundDocumentsViewModel
    {
        public string FundName { get; set; } = string.Empty;
        public string TickerCode { get; set; } = string.Empty;
        public List<DocumentJsonItem> Factsheets { get; set; } = new();
        public List<DocumentJsonItem> Prospectus { get; set; } = new();
        public List<DocumentJsonItem> Reports { get; set; } = new();
        public string RawJson { get; set; } = string.Empty;
    }
}
