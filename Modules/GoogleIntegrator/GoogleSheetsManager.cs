using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using MongoDB.Driver.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Modules.GoogleIntegrator
{
    public class GoogleSheetsManager : IGoogleSheetsManager
    {
        private readonly UserCredential _credential;

        public GoogleSheetsManager(UserCredential credential)
        {
            _credential = credential;
        }

        public async Task<Spreadsheet> CreateNewAsync(string documentName)
        {
            if (string.IsNullOrEmpty(documentName))
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            using (var sheetsService = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer() { HttpClientInitializer = _credential}))
            {
                var documentCreationRequest = sheetsService.Spreadsheets.Create(new Spreadsheet()
                {
                    Sheets = new List<Sheet>()
                    {
                        new Sheet()
                        {
                            Properties = new SheetProperties()
                            {
                                Title = documentName
                            }
                        }
                    },

                    Properties = new SpreadsheetProperties()
                    {
                        Title = documentName
                    }
                });

                return await documentCreationRequest.ExecuteAsync();
            }
        }

        public Spreadsheet GetSpreadSheet(string googleSpreadSheetIdentifier)
        {
            if (string.IsNullOrEmpty(googleSpreadSheetIdentifier))
            {
                throw new ArgumentNullException(nameof(googleSpreadSheetIdentifier));
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
                return sheetsService.Spreadsheets.Get(googleSpreadSheetIdentifier).Execute();
        }

        public ValueRange GetSingleValue(string googleSpreadSheetIdentifier, string valueRange)
        {
            if (string.IsNullOrEmpty(googleSpreadSheetIdentifier))
            {
                throw new ArgumentNullException(nameof(googleSpreadSheetIdentifier));
            }
            if (string.IsNullOrEmpty(valueRange))
            {
                throw new ArgumentNullException(nameof(valueRange));
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var getValueRequest = sheetsService.Spreadsheets.Values.Get(googleSpreadSheetIdentifier, valueRange);
                return getValueRequest.Execute();
            }
        }

        public BatchGetValuesResponse GetMultipleValues(string googleSpreadSheetIdentifier, string[] ranges)
        {
            if (string.IsNullOrEmpty(googleSpreadSheetIdentifier))
            {
                throw new ArgumentNullException(nameof(googleSpreadSheetIdentifier));
            }
            if (ranges == null || ranges.Length == 0)
            {
                throw new ArgumentNullException(nameof(ranges));
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var getValueRequest = sheetsService.Spreadsheets.Values.BatchGet(googleSpreadSheetIdentifier);
                getValueRequest.Ranges = ranges;
                return getValueRequest.Execute();
            }
        }

        public UpdateValuesResponse UpdateSingleCell(string googleSpreadSheetIdentifier, string range)
        {
            if (string.IsNullOrEmpty(googleSpreadSheetIdentifier))
            {
                throw new ArgumentNullException(nameof(googleSpreadSheetIdentifier));
            }
            if (range == null || range.Length == 0)
            {
                throw new ArgumentNullException(nameof(range));
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                ValueRange valueRange = new ValueRange();
                valueRange.MajorDimension = "COLUMNS";//"ROWS";//COLUMNS

                var oblist = new List<object>() { "My Cell Text" };
                valueRange.Values = new List<IList<object>> { oblist };

                SpreadsheetsResource.ValuesResource.UpdateRequest update = sheetsService.Spreadsheets.Values.Update(valueRange, googleSpreadSheetIdentifier, range);
                update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                UpdateValuesResponse result = update.Execute();
                Console.WriteLine(result.ToString());

                return result;
            }
        }

        public AppendValuesResponse AddRow(string googleSpreadSheetIdentifier, string range, List<object> dataList)
        {
            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                // Specifying Column Range for reading...
                //var range = $"{sheet}!A:E";
                var valueRange = new ValueRange();
                // Data for another Student...
                //var oblist = new List<object>() { "Harry", "80", "77", "62", "98" };
                valueRange.Values = new List<IList<object>> { dataList };
                // Append the above record...
                var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, googleSpreadSheetIdentifier, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
                var appendReponse = appendRequest.Execute();
                return appendReponse;
            }
        }

        public Task<Spreadsheet> GetSpreadSheetAsync(string googleSpreadSheetIdentifier)
        {
            if (string.IsNullOrEmpty(googleSpreadSheetIdentifier))
            {
                throw new ArgumentNullException(nameof(googleSpreadSheetIdentifier));
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
                return sheetsService.Spreadsheets.Get(googleSpreadSheetIdentifier).ExecuteAsync();
        }

        public Task<ValueRange> GetSingleValueAsync(string googleSpreadSheetIdentifier, string valueRange)
        {
            if (string.IsNullOrEmpty(googleSpreadSheetIdentifier))
            {
                throw new ArgumentNullException(nameof(googleSpreadSheetIdentifier));
            }
            if (string.IsNullOrEmpty(valueRange))
            {
                throw new ArgumentNullException(nameof(valueRange));
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var getValueRequest = sheetsService.Spreadsheets.Values.Get(googleSpreadSheetIdentifier, valueRange);
                return getValueRequest.ExecuteAsync();
            }
        }

        public Task<BatchGetValuesResponse> GetMultipleValuesAsync(string googleSpreadSheetIdentifier, string[] ranges)
        {
            if (string.IsNullOrEmpty(googleSpreadSheetIdentifier))
            {
                throw new ArgumentNullException(nameof(googleSpreadSheetIdentifier));
            }
            if (ranges == null || ranges.Length == 0)
            {
                throw new ArgumentNullException(nameof(ranges));
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = _credential }))
            {
                var getValueRequest = sheetsService.Spreadsheets.Values.BatchGet(googleSpreadSheetIdentifier);
                var getValueRequest1 = sheetsService.Spreadsheets;
                getValueRequest.Ranges = ranges;
                return getValueRequest.ExecuteAsync();
            }
        }
    }
}
