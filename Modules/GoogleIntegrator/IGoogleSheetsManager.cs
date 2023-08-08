using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Modules.GoogleIntegrator
{
    public interface IGoogleSheetsManager
    {
        Task<Spreadsheet> CreateNewAsync(string documentName);
        Task<Spreadsheet> GetSpreadSheetAsync(string googleSpreadSheetIdentifier);
        Task<ValueRange> GetSingleValueAsync(string googleSpreadSheetIdentifier, string valueRange);
        Task<BatchGetValuesResponse> GetMultipleValuesAsync(string googleSpreadSheetIdentifier, string[] ranges);
    }
}
