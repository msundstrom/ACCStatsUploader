using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ACCStatsUploader.GoogleAPI {

    public enum Dimension {
        ROWS,
        COLUMNS
    }

    public enum MergeType {
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    public enum CellVerticalAlignment {
        TOP,
        MIDDLE,
        BOTTOM
    }

    public enum CellHorizontalAlignment {
        LEFT,
        CENTER,
        RIGHT
    }

    public class BaseRequestFactory {

        // Add requests
        public AddSheetRequest addSheet(
            string sheetTitle,
            bool hidden
        ) {
            return new AddSheetRequest() {
                Properties = new SheetProperties {
                    Title = sheetTitle,
                    Hidden = hidden
                }
            };
        }

        public AppendCellsRequest addRow(
            int sheetId,
            IList<object> rowData
        ) {
            return new AppendCellsRequest() {
                SheetId = sheetId,
                Rows = new[] { createRowData(rowData) },
                Fields = "*",
            };
        }

        public InsertDimensionRequest insertDimension(
            int sheetId,
            Dimension dimension,
            int startIndex,
            int count
        ) {
            return new InsertDimensionRequest() {
                Range = new DimensionRange {
                    SheetId = sheetId,
                    Dimension = convertDimension(dimension),
                    StartIndex = startIndex,
                    EndIndex = startIndex + count
                },
                InheritFromBefore = true
            };
        }

        public AppendDimensionRequest appendDimension(
            int sheetId,
            Dimension dimension,
            int count
        ) {
            return new AppendDimensionRequest {
                SheetId = sheetId,
                Dimension = convertDimension(dimension),
                Length = count-1
            };
        }

        // Update requests
        public UpdateCellsRequest updateCell(
            int sheetId,
            object cellData,
            int column,
            int row,
            TextFormat? textFormat = null,
            CellFormat? cellFormat = null
        ) {
            return new UpdateCellsRequest() {
                Rows = new[] { createRowData(new List<object>{ cellData }, textFormat, cellFormat) },
                Range = new GridRange() {
                    SheetId = sheetId,
                    StartColumnIndex = column,
                    EndColumnIndex = column + 1,
                    StartRowIndex = row,
                    EndRowIndex = row + 1,
                },
                Fields = "*"
            };
        }

        public UpdateCellsRequest updateCells(
            int sheetId,
            IList<object> rowData,
            int column,
            int row,
            TextFormat? textFormat = null,
            CellFormat? cellFormat = null
        ) {
            return new UpdateCellsRequest() {
                Rows = new[] { createRowData(rowData, textFormat, cellFormat) },
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

        public UpdateCellsRequest updateColumn(
            int sheetId,
            IList<object> column,
            int columnIndex,
            int startRow,
            TextFormat? textFormat = null,
            CellFormat? cellFormat = null,
            string? fields = null
        ) {
            var colData = column.Select(val => new List<object> { val });

            return new UpdateCellsRequest() {
                Rows = createColumnData(column, textFormat, cellFormat),
                Range = new GridRange() {
                    SheetId = sheetId,
                    StartColumnIndex = columnIndex,
                    StartRowIndex = startRow,
                    EndColumnIndex= columnIndex + 1,
                    EndRowIndex= startRow + colData.Count(),
                },
                Fields = fields ?? "*",
            };
        }

        public PasteDataRequest pasteData(
            int sheetId,
            IList<object> rowData,
            int column,
            int row
        ) {
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


        // Delete requests
        public DeleteDimensionRequest deleteDimension(
            int sheetId,
            Dimension dimension,
            int startIndex,
            int? endIndex = null
        ) {
            return new DeleteDimensionRequest() {
                Range = new DimensionRange {
                    SheetId = sheetId,
                    Dimension = convertDimension(dimension),
                    StartIndex = startIndex,
                    EndIndex = endIndex
                }
            };
        }

        // Formatting
        public MergeCellsRequest mergeRange(
            int sheetId,
            MergeType mergeType,
            int startColumn,
            int startRow,
            int endColumn,
            int endRow
        ) {
            return new MergeCellsRequest {
                Range = new GridRange {
                    SheetId = sheetId,
                    StartRowIndex = startRow,
                    StartColumnIndex = startColumn,
                    EndRowIndex = endRow,
                    EndColumnIndex = endColumn
                },
                MergeType = convertMergeType(mergeType)  
            };
        }

        public UpdateDimensionPropertiesRequest setDimensionSize(
            int sheetId,
            int width,
            Dimension dimension,
            int start,
            int? end = null
        ) {
            return new UpdateDimensionPropertiesRequest {
                Properties = new DimensionProperties {
                    PixelSize = width,
                },
                Range = new DimensionRange {
                    SheetId = sheetId,
                    Dimension = convertDimension(dimension),
                    StartIndex = start,
                    EndIndex = end
                },
                Fields = "*"
            };
        }

        public UpdateSheetPropertiesRequest freezeRows(
            int sheetId,
            int rowCount
        ) {
            return new UpdateSheetPropertiesRequest {
                Properties = new SheetProperties {
                    SheetId = sheetId,
                    GridProperties = new GridProperties {
                        FrozenRowCount = rowCount,
                    }
                },
                Fields = "gridProperties.frozenRowCount"
            };
        }

        public UpdateSheetPropertiesRequest freezeColumns(
            int sheetId,
            int colCount
        ) {
            return new UpdateSheetPropertiesRequest {
                Properties = new SheetProperties {
                    SheetId = sheetId,
                    GridProperties = new GridProperties {
                        FrozenColumnCount = colCount,
                    }
                },
                Fields= "gridProperties.frozenColumnCount"
            };
        }

        public UpdateBordersRequest setRightBorder(
              int sheetId,
              Border border,
              int? startRow,
              int? endRow,
              int? startCol,
              int? endCol
        ) {
            return new UpdateBordersRequest {
                Range = new GridRange {
                    SheetId = sheetId,
                    StartRowIndex = startRow,
                    EndRowIndex = endRow,
                    StartColumnIndex = startCol,
                    EndColumnIndex = endCol
                },
                Right = border,
            };
        }

        // Misc

        public AutoResizeDimensionsRequest autoResizeDimensions(
            int sheetId, 
            Dimension dimension
        ) {
            return new AutoResizeDimensionsRequest {
                Dimensions = new DimensionRange {
                    SheetId = sheetId,
                    Dimension = convertDimension(dimension)
                }
            };
        }

        // Private utility
        private string convertDimension(Dimension dimension) {
            switch (dimension) {
                case Dimension.ROWS:
                    return "ROWS";
                case Dimension.COLUMNS:
                    return "COLUMNS";
                default:
                    throw new Exception("Invalid dimension!");
            }
        }

        private string convertMergeType(MergeType mergeType) {
            switch (mergeType) {
                case MergeType.VERTICAL:
                    return "MERGE_COLUMNS";
                case MergeType.HORIZONTAL:
                    return "MERGE_ROWS";
                case MergeType.ALL:
                    return "MERGE_ALL";
                default:
                    return "ALL";
            }
        }

        private ExtendedValue createValueFrom(object inputValue) {
            var extendedValue = new ExtendedValue();

            if (inputValue.GetType() == typeof(string)) {
                extendedValue = new ExtendedValue {
                    StringValue = (string)inputValue
                };
            } else if (inputValue.GetType() == typeof(int) || inputValue.GetType() == typeof(float)) {
                extendedValue = new ExtendedValue {
                    NumberValue = Convert.ToDouble(inputValue)
                };
            } else if (inputValue.GetType() == typeof(double)) {
                extendedValue = new ExtendedValue {
                    NumberValue = (double)inputValue
                };
            } else if (inputValue.GetType() == typeof(Formula)) {
                extendedValue = new ExtendedValue {
                    FormulaValue = ((Formula)inputValue).value
                };
            }

            return extendedValue;
        }

        private RowData createRowData(
            IList<object> rowData, 
            TextFormat? textFormat = null,
            CellFormat? cellFormat = null
        ) {
            RowData row = new RowData();
            row.Values = new List<CellData>();
            var cellDataArray = rowData.Select(test => {
                var cellData = new CellData();
                cellData.UserEnteredFormat = cellFormat;

                if (textFormat != null) {
                    cellData.TextFormatRuns = new List<TextFormatRun> {
                        new TextFormatRun() {
                            Format = textFormat
                        }
                    };
                }

                cellData.UserEnteredValue = createValueFrom(test);

                return cellData;
            });

            foreach (var cellData in cellDataArray) {
                row.Values.Add(cellData);
            }

            return row;
        }

        private IList<RowData> createColumnData(
            IList<object> data,
            TextFormat? textFormat = null,
            CellFormat? cellFormat = null
        ) {
            var colData = data.Select(val => new List<object> { val });

            var output = new List<RowData>();

            foreach (var row in data) {
                var cellData = new CellData();
                cellData.UserEnteredFormat = cellFormat;

                if (textFormat != null) {
                    cellData.TextFormatRuns = new List<TextFormatRun> {
                        new TextFormatRun() {
                            Format = textFormat
                        }
                    };
                }

                cellData.UserEnteredValue = createValueFrom(row);

                output.Add(new RowData {
                    Values = new List<CellData> { cellData }
                });
            }

            return output;
        }
    }
}
