using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    public class SheetsAPIController {
        string[] Scopes = { SheetsService.Scope.Spreadsheets };
        SheetsService sheetService;
        String documentId = "";

        public bool isConnected = false;

        public async void initializeGoogleApi(
            String applicationName,
            String documentId
        ) {
            this.documentId = documentId;
            UserCredential credential;

            // Load client secrets.
            using (var stream =
                   new FileStream("credentials.json", FileMode.Open, FileAccess.Read)) {
                /* The file token.json stores the user's access and refresh tokens, and is created
                 automatically when the authorization flow completes for the first time. */
                string credPath = "token.json";
                var test = new GoogleClientSecrets();
       
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }


            this.sheetService = new SheetsService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });
        }

        public async Task<IList<Google.Apis.Sheets.v4.Data.Sheet>> getSheets() {
            var sheetRequest = sheetService.Spreadsheets.Get(documentId);
            Spreadsheet spreadSheet = await sheetRequest.ExecuteAsync();

            return spreadSheet.Sheets;
        }

        public async Task<int?> createSheet(Sheet sheetToAdd) {
            var addSheetRequest = new AddSheetRequest();
            addSheetRequest.Properties = new SheetProperties();
            addSheetRequest.Properties.Title = sheetToAdd.sheetTitle;

            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            batchUpdateSpreadsheetRequest.Requests.Add(new Request { AddSheet = addSheetRequest });

            var batchUpdateRequest = this.sheetService.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, this.documentId);
            try {
                var result = await batchUpdateRequest.ExecuteAsync();
                var sheetId = result.Replies[0].AddSheet.Properties.SheetId;

                return sheetId;
            } catch (System.Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.GetType());
                if (e is Google.GoogleApiException) {
                    Google.GoogleApiException apiException = (Google.GoogleApiException)e;

                    // sheet already exists
                    if (apiException.HttpStatusCode == System.Net.HttpStatusCode.BadRequest) {
                        return null;
                    }
                }

                return null;
            }
        }

        public async Task setupSheet(int sheetId, int columnsToAdd) {
            var requests = new List<Request> {
                new Request {
                    DeleteDimension = new DeleteDimensionRequest {
                        Range = new DimensionRange {
                            StartIndex = 1,
                            Dimension = "COLUMNS",
                            SheetId = sheetId,
                        }
                    }
                },
                new Request {
                    AppendDimension = new AppendDimensionRequest() {
                        SheetId = sheetId,
                        Dimension = "COLUMNS",
                        Length = columnsToAdd - 1
                    }
                }
            };

            SpreadsheetsResource.BatchUpdateRequest batchUpdateRequest = buildBatchRequest(
                requests
            );

            try {
                await batchUpdateRequest.ExecuteAsync();
            } catch (System.Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.GetType());
            }
        }

        public async Task appendRow(
            int sheetId,
            IList<object> rowData,
            TextFormat? format = null,
            bool autoSize = false
        ) {
            var requests = new List<Request> {
                new Request {
                    AppendCells = createAppendRowRequest(
                        sheetId,
                        rowData,
                        format
                    )
                }
            };

            if (autoSize) {
                requests.Add(
                    new Request {
                        AutoResizeDimensions = createAutoResizeDimensionsRequest(
                            sheetId, 
                            "COLUMNS"
                        )
                    }
                );
            }

            SpreadsheetsResource.BatchUpdateRequest batchUpdateRequest = buildBatchRequest(
                requests
            );

            try {
                await batchUpdateRequest.ExecuteAsync();
            } catch (System.Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.GetType());
            }
        }

        public async Task insertEmptyColumns(int sheetId, int count) {
            SpreadsheetsResource.BatchUpdateRequest batchUpdateRequest = buildBatchRequest(
                new List<Request> {
                    new Request {
                        AppendDimension = new AppendDimensionRequest() {
                            SheetId = sheetId,
                            Dimension = "COLUMNS",
                            Length = count
                        }
                    }
                }
            );

            try {
                await batchUpdateRequest.ExecuteAsync();
            } catch (System.Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.GetType());
            }
        }

        private AppendCellsRequest createAppendRowRequest(int sheetId, IList<object> rowData, TextFormat? format = null) {
            RowData row = new RowData();
            row.Values = new List<CellData>();
            var cellDataArray = rowData.Select(test => {
                var cellData = new CellData();

                if (format != null) {
                    cellData.TextFormatRuns = new List<TextFormatRun> {
                        new TextFormatRun() {
                            Format = format
                        }
                    };
                }

                if (test.GetType() == typeof(string)) {
                    cellData.UserEnteredValue = new ExtendedValue {
                        StringValue = (string)test
                    };
                } else if (test.GetType() == typeof(int) || test.GetType() == typeof(float)) {
                    cellData.UserEnteredValue = new ExtendedValue {
                        NumberValue = Convert.ToDouble(test)
                    };
                } else if (test.GetType() == typeof(double)) {
                    cellData.UserEnteredValue = new ExtendedValue {
                        NumberValue = (double)test
                    };
                }

                return cellData;
            });
            
            foreach (var cellData in cellDataArray) {
                row.Values.Add(cellData);
            }

            AppendCellsRequest appendCellsRequest = new AppendCellsRequest();
            appendCellsRequest.Rows = new[] { row };
            appendCellsRequest.Fields = "*";
            appendCellsRequest.SheetId = sheetId;

            return appendCellsRequest;
        }

        private AutoResizeDimensionsRequest createAutoResizeDimensionsRequest(int sheetId, string dimension) {
            AutoResizeDimensionsRequest autoResizeDimensionsRequest = new AutoResizeDimensionsRequest() {
                Dimensions = new DimensionRange() {
                    SheetId = sheetId,
                    Dimension = dimension
                }
            };

            return autoResizeDimensionsRequest;
        }

        private SpreadsheetsResource.BatchUpdateRequest buildBatchRequest(IList<Request> requests) {
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = requests;
            var batchUpdateRequest = this.sheetService.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, this.documentId);

            return batchUpdateRequest;
        }
    }
}