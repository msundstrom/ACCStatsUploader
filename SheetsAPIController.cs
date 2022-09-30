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
using System.Threading.Tasks;
using Google.Apis.Util.Store;
using System.Windows;
using System.DirectoryServices.ActiveDirectory;
using CommandLine;

namespace ACCStatsUploader {
    public class SheetsAPIController {
        string[] Scopes = { SheetsService.Scope.Spreadsheets };
        SheetsService sheetService;
        String documentId = "";

        public bool isConnected = false;

        private string asf = "replace_me";

        public static Stream GenerateStreamFromString(string s) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public async void initializeGoogleApi(
            String applicationName,
            String documentId
        ) {
            this.documentId = documentId;
            UserCredential credential;
            Stream stream;

            if (asf != "replace_me") {
                stream = GenerateStreamFromString(asf);
            } else {
                // Load client secrets.
                stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read);
            }

            /* The file token.json stores the user's access and refresh tokens, and is created
                 automatically when the authorization flow completes for the first time. */
            string credPath = "token.json";
            var test = new GoogleClientSecrets();

            if (stream == null) {
                MessageBox.Show("Missing credentials!");
                return;
            }

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;

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

        public async Task insertRow(
            int sheetId,
            IList<object> rowData,
            int row
        ) {
            var requests = new List<Request> {
                new Request {
                    InsertDimension = createInsertRowRequest(sheetId, row-1)
                },
                new Request {
                    UpdateCells = createUpdateCellsRequest(sheetId, rowData, 0, row)
                }
            };

            SpreadsheetsResource.BatchUpdateRequest batchUpdateRequest = buildBatchRequest(
                requests
            );

            try {
                var response = await batchUpdateRequest.ExecuteAsync();
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

        public async Task removeRows(int sheetId) {
            var requests = new List<Request> {
                new Request {
                    DeleteDimension = new DeleteDimensionRequest {
                        Range = new DimensionRange {
                            StartIndex = 1,
                            Dimension = "ROWS",
                            SheetId = sheetId,
                        }
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

        private AppendCellsRequest createAppendRowRequest(int sheetId, IList<object> rowData, TextFormat? format = null) {
            AppendCellsRequest appendCellsRequest = new AppendCellsRequest();
            appendCellsRequest.Rows = new[] { createRowData(rowData) };
            appendCellsRequest.Fields = "*";
            appendCellsRequest.SheetId = sheetId;

            return appendCellsRequest;
        }

        private InsertDimensionRequest createInsertRowRequest(int sheetId, int position) {
            return new InsertDimensionRequest() {
                Range = new DimensionRange {
                    SheetId = sheetId,
                    Dimension = "ROWS",
                    StartIndex = 1,
                    EndIndex = 2
                },
                InheritFromBefore = true,
            };
        }

        private UpdateCellsRequest createUpdateCellsRequest(int sheetId, IList<object> rowData, int column, int row) {
            return new UpdateCellsRequest() {
                Rows = new[] { createRowData(rowData) },
                Range = new GridRange() {
                    SheetId = sheetId,
                    StartColumnIndex = column,
                    EndColumnIndex = column + rowData.Count,
                    StartRowIndex = row,
                    EndRowIndex = row + 1,
                },
                Fields = "*"
            };
        }

        private PasteDataRequest createPasteDataRequest(int sheetId, IList<object> rowData, int column, int row) {
            return new PasteDataRequest() {
                Data = String.Join(";", rowData.ToArray()),
                Type = "PASTE_VALUES",
                Delimiter = ";",
                Coordinate = new GridCoordinate() {
                    SheetId = sheetId,
                    RowIndex = 1
                }
            };
        }

        private RowData createRowData(IList<object> rowData, TextFormat? format = null) {
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

            return row;
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