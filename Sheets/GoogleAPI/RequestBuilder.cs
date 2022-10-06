using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ACCStatsUploader.GoogleAPI {
    public class SheetRequest {
        public SheetsService sheetService;
        public string documentId;
        private IList<Request> requests = new List<Request>();

        public SheetRequest(SheetsService sheetService, string documentId) {
            this.sheetService = sheetService;
            this.documentId = documentId;
        }

        public void addRequest(Request request) {
            requests.Add(request);
        }

        public void addRequests(IList<Request> newRequests) {
            requests = requests.Concat(newRequests).ToList();
        }

        public async Task execute() {
            var request = buildBatchRequest(requests);

            try {
                await request.ExecuteAsync();
            } catch (System.Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.GetType());
            }
        }

        private SpreadsheetsResource.BatchUpdateRequest buildBatchRequest(IList<Request> requests) {
            //return sheetService.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest {
            //    Requests = requests,
            //}, documentId);

            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = requests;
            var batchUpdateRequest = this.sheetService.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, this.documentId);

            return batchUpdateRequest;
        }
    }
}
