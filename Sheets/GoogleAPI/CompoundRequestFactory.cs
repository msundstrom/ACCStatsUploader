using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.GoogleAPI {
    public class CompoundRequestFactory {

        private BaseRequestFactory requestFactory = new BaseRequestFactory();

        public IList<Request> clearSheet(int sheetId) {
            return new List<Request>() {
                requestFactory.deleteDimension(
                    sheetId,
                    Dimension.ROWS,
                    1
                ).asRequest(),
                requestFactory.deleteDimension(
                    sheetId,
                    Dimension.COLUMNS,
                    1
                ).asRequest()
            };
        }

        public IList<Request> insertRow(
            int sheetId,
            int row,
            IList<object> rowData,
            int? startColumn = null
        ) {
            return new List<Request>() {
                requestFactory.insertDimension(
                    sheetId,
                    Dimension.ROWS,
                    row,
                    1
                ),
                requestFactory.updateCells(
                    sheetId,
                    rowData,
                    startColumn ?? 0,
                    row
                ).asRequest()
            };
        }
    }
}
