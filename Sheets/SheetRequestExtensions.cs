using ACCStatsUploader.Converters;
using ACCStatsUploader.GoogleAPI;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ACCStatsUploader {
    using IRequestList = IList<Request>;
    using RequestList = List<Request>;
    using ICells = IList<Cell>;
    using Cells = List<Cell>;

    public static class SheetExtensions {

        public async static Task createSheet(this Sheet sheet, SheetsAPIController gsController) {
            var lapSheetId = await gsController.createSheet(sheet);
            if (lapSheetId == null) {
                MessageBox.Show("Creating "+ sheet.sheetTitle +" failed!");
                throw new SheetCreationFailedException();
            }
            sheet.sheetId = (int)lapSheetId!;
        }

        public static IRequestList clearSheet(this Sheet sheet) {
            var baseFactory = new BaseRequestFactory();

            return new RequestList {
                baseFactory.deleteDimension(sheet.sheetId, Dimension.ROWS, 1).asRequest(),
                baseFactory.deleteDimension(sheet.sheetId, Dimension.COLUMNS, 1).asRequest(),
            };
        }

        public static Request addEmptyColumns(this Sheet sheet, int nrOfColumns, int? startingAt = null) {
            var baseFactory = new BaseRequestFactory();

            return baseFactory.insertDimension(sheet.sheetId, Dimension.COLUMNS, startingAt ?? 1, nrOfColumns);
        }

        public static Request addEmptyRows(this Sheet sheet, int nrOfRows, int? startingAt = null) {
            var baseFactory = new BaseRequestFactory();

            return baseFactory.insertDimension(sheet.sheetId, Dimension.ROWS, startingAt ?? 1, nrOfRows);
        }

        public static Request appendRow(this Sheet sheet, ICells cells) {
            var baseFactory = new BaseRequestFactory();

            return baseFactory.addRow(sheet.sheetId, cells);
        }

        public static Request mergeRange(this Sheet sheet, CellRange range, MergeType mergeType) {
            var baseFactory = new BaseRequestFactory();
            return baseFactory.mergeRange(sheet.sheetId, mergeType, range.startCol, range.startRow, range.endCol, range.endRow).asRequest();
        }

        public static Request updateCell(this Sheet sheet, CellRange range, Cell cell, string? fields = null) {
            var baseFactory = new BaseRequestFactory();
            return baseFactory.updateCell(sheet.sheetId, cell, (int)range.startCol!, (int)range.startRow!, fields).asRequest();
        }

        public static Request updateCells(this Sheet sheet, CellRange range, ICells cells, string? fields = null) {
            var baseFactory = new BaseRequestFactory();
            return baseFactory.updateCells(sheet.sheetId, cells, range, fields).asRequest();
        }

        public static Request updateColumn(this Sheet sheet, CellRange range, ICells cells, string? fields = null) {
            var baseFactory = new BaseRequestFactory();
            return baseFactory.updateColumn(sheet.sheetId, cells, range.startCol ?? 1, range.startRow ?? 1).asRequest();
        }

        public static Request resize(this Sheet sheet, Dimension dimension, CellRange range, int pixelSize) {
            var baseFactory = new BaseRequestFactory();

            int startIndex = -1;
            int? endIndex = null;

            switch (dimension) {
                case Dimension.ROWS:
                    startIndex = range.startRow ?? 0;
                    endIndex = range.endRow;
                    break;
                case Dimension.COLUMNS:
                    startIndex = range.startCol ?? 0;
                    endIndex = range.endCol;
                    break;
            }

            return baseFactory.setDimensionSize(sheet.sheetId, pixelSize, dimension, startIndex, endIndex).asRequest();
        }

        public static Request setBorder(this Sheet sheet, BorderStyle borderStyle, BorderEdge edge, CellRange range) {
            var baseFactory = new BaseRequestFactory();
            return baseFactory.setBorder(sheet.sheetId, borderStyle, edge, range).asRequest();
        }

        public static Request freezeRows(this Sheet sheet, int rowCount) {
            var baseFactory = new BaseRequestFactory();
            return baseFactory.freezeRows(sheet.sheetId, rowCount).asRequest();
        }

        public static Request freezeColumns(this Sheet sheet, int colCount) {
            var baseFactory = new BaseRequestFactory();
            return baseFactory.freezeColumns(sheet.sheetId, colCount).asRequest();
        }
    }
}
