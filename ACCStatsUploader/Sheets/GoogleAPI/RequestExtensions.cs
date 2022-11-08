using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.GoogleAPI {
    public static class RequestExtensions {
        public static Request asRequest(this AddSheetRequest request) {
            return new Request {
                AddSheet = request
            };
        }

        public static Request asRequest(this AppendCellsRequest request) {
            return new Request {
                AppendCells = request
            };
        }

        public static Request asRequest(this InsertDimensionRequest request) {
            return new Request {
                InsertDimension = request
            };
        }

        public static Request asRequest(this UpdateCellsRequest request) {
            return new Request {
                UpdateCells = request
            };
        }

        public static Request asRequest(this PasteDataRequest request) {
            return new Request {
                PasteData = request
            };
        }

        public static Request asRequest(this DeleteDimensionRequest request) {
            return new Request {
                DeleteDimension = request
            };
        }

        public static Request asRequest(this AutoResizeDimensionsRequest request) {
            return new Request {
                AutoResizeDimensions = request
            };
        }

        public static Request asRequest(this AppendDimensionRequest request) {
            return new Request {
                AppendDimension = request
            };
        }

        public static Request asRequest(this MergeCellsRequest request) {
            return new Request {
                MergeCells = request
            };
        }

        public static Request asRequest(this UpdateDimensionPropertiesRequest request) {
            return new Request {
                UpdateDimensionProperties = request
            };
        }

        public static Request asRequest(this UpdateSheetPropertiesRequest request) {
            return new Request {
                UpdateSheetProperties = request
            };
        }

        public static Request asRequest(this UpdateBordersRequest request) {
            return new Request {
                UpdateBorders = request
            };
        }
    }
}
