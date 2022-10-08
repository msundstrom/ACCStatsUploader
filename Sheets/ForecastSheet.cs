using ACCStatsUploader.GoogleAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;
using System.Drawing;
using Color = Google.Apis.Sheets.v4.Data.Color;
using ACCStatsUploader.Converters;

namespace ACCStatsUploader {
    using IRequestList = IList<Request>;
    using RequestList = List<Request>;
    using ICells = IList<Cell>;
    using Cells = List<Cell>;
    using ColorConverter = Converters.ColorConverter;

    public class ForecastSheet : Sheet {

        public string sheetTitle {
            get {
                return SHEET_NAMES.FORECAST;
            }
        }
        public int sheetId { get; set; }
        public bool hidden {
            get {
                return false;
            }
        }

        private SheetsAPIController gsController { get; set; }

        public ForecastSheet(SheetsAPIController gsController) {
            this.gsController = gsController;   
        }

        public async Task create() {
            await this.createSheet(gsController);
            await setup();
        }

        private async Task setup() {
            var setupRequest = gsController.createSheetRequest();

            setupRequest.addRequests(this.clearSheet());
            setupRequest.addRequest(this.addEmptyColumns(3));
            setupRequest.addRequest(this.addEmptyRows(32));

            // main title merge
            setupRequest.addRequest(this.mergeRange(new CellRange {
                startCol = 0,
                startRow = 0,
                endCol = 4,
                endRow = 1,
            }, MergeType.HORIZONTAL));

            // Ingame clock merge
            setupRequest.addRequest(this.mergeRange(new CellRange {
                startCol = 0,
                startRow = 1,
                endCol = 2,
                endRow = 2,
            }, MergeType.HORIZONTAL));

            // Current weather merge
            setupRequest.addRequest(this.mergeRange(new CellRange {
                startCol = 2,
                startRow = 2,
                endCol = 4,
                endRow = 3,
            }, MergeType.HORIZONTAL));

            // 30 min weather slots merge
            setupRequest.addRequest(this.mergeRange(new CellRange {
                startCol = 2,
                startRow = 13,
                endCol = 4,
                endRow = 33,
            }, MergeType.HORIZONTAL));


            // add first 10 min values based on 10
            var firstTenMinValuesBasedOnTen = new Cells();
            for (int i = 11; i >= 2; i--) {
                firstTenMinValuesBasedOnTen.Add(
                    new Cell {
                        value = new Formula {
                            value = "=IFERROR(INDIRECT(\"weather_data!$H$" + i + "\");\"\")"
                        },
                        format = new CellFormat {
                            HorizontalAlignment = AlignmentConverter.horizontal(CellHorizontalAlignment.CENTER)
                        }
                    }
                );
            }
            setupRequest.addRequest(this.updateColumn(new CellRange {
                startRow = 3,
                startCol = 2,
            }, firstTenMinValuesBasedOnTen));

            // add first 10 min values based on 30
            var firstTenMinValuesBasedOnThirty = new Cells();
            for (int i = 31; i >= 22; i--) {
                firstTenMinValuesBasedOnThirty.Add(
                    new Cell {
                        value = new Formula {
                            value = "=IFERROR(INDIRECT(\"weather_data!$I$" + i + "\");\"\")"
                        },
                        format = new CellFormat {
                            HorizontalAlignment = AlignmentConverter.horizontal(CellHorizontalAlignment.CENTER)
                        }
                    }
                );
            }
            setupRequest.addRequest(this.updateColumn(new CellRange {
                startRow = 3,
                startCol = 3,
            }, firstTenMinValuesBasedOnThirty));

            // add last 20 min values based on 30
            var lastTwentyMinValuesBasedOnThirty = new Cells();
            for (int i = 21; i >= 2; i--) {
                lastTwentyMinValuesBasedOnThirty.Add(
                    new Cell {
                        value = new Formula {
                            value = "=IFERROR(INDIRECT(\"weather_data!$I$" + i + "\");\"\")"
                        },
                        format = new CellFormat {
                            HorizontalAlignment = AlignmentConverter.horizontal(CellHorizontalAlignment.CENTER)
                        }
                    }
                );
            }
            setupRequest.addRequest(this.updateColumn(new CellRange {
                startRow = 13,
                startCol = 2,
            }, lastTwentyMinValuesBasedOnThirty));

            // Setup the time labels
            var timePreviewLabelColumnValues = new Cells();
            var timePreviewColumnValues = new Cells();
            for (int i = 0; i <= 30; i++) {
                if (i == 0) {
                    timePreviewLabelColumnValues.Insert(0, new Cell { value = "now", format = CellFormats.centered });
                    timePreviewColumnValues.Add(new Cell { 
                        value = new Formula { value = "=IFERROR(TIME(0;0;INDIRECT(\"weather_data!$B$2\"));\"\")" },
                        format = CellFormats.centeredWithNumberFormat(NumberFormats.clock)
                    });
                } else {
                    timePreviewLabelColumnValues.Add(new Cell { value = "in " + i + " min", format = CellFormats.centered });
                    timePreviewColumnValues.Add(new Cell { 
                        value = new Formula { value = "=IFERROR(TIME(0;0;INDIRECT(\"weather_data!$B$2\") + " + i * 60 + ");\"\")"},
                        format = CellFormats.centeredWithNumberFormat(NumberFormats.clock)
                    });
                }
            }
            setupRequest.addRequest(this.updateColumn(new CellRange {
                startRow = 2,
                startCol = 0,
            }, timePreviewLabelColumnValues));
            setupRequest.addRequest(this.updateColumn(new CellRange {
                startRow = 2,
                startCol = 1,
            }, timePreviewColumnValues));

            // setup current weather
            setupRequest.addRequest(this.updateCell(
                new CellRange { startRow = 2, startCol = 2 },
                new Cell { value = new Formula { value = "=IFERROR(INDIRECT(\"weather_data!$C$2\");\"\")" }, format = CellFormats.centered }
            ));

            // Setup static texts
            setupRequest.addRequest(this.updateCell(new CellRange {
                startRow = 0,
                startCol = 0
            }, new Cell {
                value = "Weather forecast",
                format = new CellFormat {
                    TextFormat = new TextFormat {
                        FontSize = 12,
                        Bold = true,
                    },
                    HorizontalAlignment = "CENTER",
                    VerticalAlignment = "MIDDLE",
                    BackgroundColor = ColorConverter.fromHex("#9bc2e6")
                }
            }));

            setupRequest.addRequest(this.updateCell(new CellRange {
                startRow = 1,
                startCol = 0
            }, new Cell {
                value = "In game clock",
                format = new CellFormat {
                    TextFormat = new TextFormat {
                        FontSize = 10,
                        Bold = true,
                    },
                    HorizontalAlignment = "CENTER",
                    VerticalAlignment = "MIDDLE",
                    BackgroundColor = ColorConverter.fromHex("#bfbfbf")
                }
            }));

            setupRequest.addRequest(this.updateCell(new CellRange {
                startRow = 1,
                startCol = 2
            }, new Cell {
                value = "Based on 10",
                format = new CellFormat {
                    TextFormat = new TextFormat {
                        FontSize = 10,
                        Bold = true,
                    },
                    HorizontalAlignment = "CENTER",
                    VerticalAlignment = "MIDDLE",
                    BackgroundColor = ColorConverter.fromHex("#bfbfbf")
                }
            }));

            setupRequest.addRequest(this.updateCell(new CellRange {
                startRow = 1,
                startCol = 3
            }, new Cell {
                value = "Based on 30",
                format = new CellFormat {
                    TextFormat = new TextFormat {
                        FontSize = 10,
                        Bold = true,
                    },
                    HorizontalAlignment = "CENTER",
                    VerticalAlignment = "MIDDLE",
                    BackgroundColor = ColorConverter.fromHex("#bfbfbf")
                }
            }));

            // Resize some rows and cols
            setupRequest.addRequest(this.resize(Dimension.ROWS, new CellRange {
                startRow = 0,
                endRow = 1
            }, 40));

            setupRequest.addRequest(this.resize(Dimension.COLUMNS, new CellRange {
                startCol = 0
            }, 100));


            await setupRequest.execute();
        }

        private IList<float> generateRgba(string backgroundColor) {
            System.Drawing.Color color = ColorTranslator.FromHtml(backgroundColor);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);

            return new List<float> {
                (float)(r / 255.0), 
                (float)(g / 255.0), 
                (float)(b / 255.0)
            };
        }
    }
}
