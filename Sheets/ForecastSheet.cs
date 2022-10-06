using ACCStatsUploader.GoogleAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;
using System.Drawing;
using Color = Google.Apis.Sheets.v4.Data.Color;

namespace ACCStatsUploader {
    public class ForecastSheet : Sheet {

        private CellFormat centeredTextFormat = new CellFormat() {
            HorizontalAlignment = "CENTER",
            VerticalAlignment = "MIDDLE"
        };

        public ForecastSheet() {
            sheetTitle = Sheet.SHEET_NAMES.FORECAST;
        }

        public async Task setup(SheetsAPIController gsController) {
            var baseFactory = new BaseRequestFactory();
            var compoundFactory = new CompoundRequestFactory();

            var request = gsController.createSheetRequest();

            // Clear sheet and set correct cols
            request.addRequests(compoundFactory.clearSheet(sheetId));
            request.addRequest(baseFactory.appendDimension(
                sheetId,
                Dimension.COLUMNS,
                4
            ).asRequest());
            request.addRequest(baseFactory.appendDimension(
                sheetId,
                Dimension.ROWS,
                34
            ).asRequest());


            request.addRequests(createMergeRequests(baseFactory));
            request.addRequests(createFormulaRequests(baseFactory));
            request.addRequests(createTimeLabelRequests(baseFactory));
            request.addRequests(createStaticTextRequests(baseFactory));
            request.addRequests(generateResizeRequests(baseFactory));

            await request.execute();
        }

        private IList<Request> createStaticTextRequests(BaseRequestFactory baseFactory) {
            var titleBackgroundColor = generateRgba("#9bc2e6");
            var subTitleBackgroundColor = generateRgba("#bfbfbf");

            return new List<Request>() {
                baseFactory.updateCells(
                    sheetId,
                    new List<object> {
                        "Weather forecast"
                    },
                    0,
                    1,
                    new TextFormat {
                        FontSize = 12,
                        Bold = true,
                    },
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE",
                        BackgroundColor = new Color {
                            Red = titleBackgroundColor[0],
                            Green = titleBackgroundColor[1],
                            Blue = titleBackgroundColor[2]
                        }
                    }
                ).asRequest(),
                baseFactory.updateCells(
                    sheetId,
                    new List<object> {
                        "In game clock"
                    },
                    0,
                    2,
                    new TextFormat {
                        Bold = true,
                    },
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE",
                        BackgroundColor = new Color {
                            Red = subTitleBackgroundColor[0],
                            Green = subTitleBackgroundColor[1],
                            Blue = subTitleBackgroundColor[2]
                        }
                    }
                ).asRequest(),
                 baseFactory.updateCells(
                    sheetId,
                    new List<object> {
                        "Based on 10"
                    },
                    2,
                    2,
                    new TextFormat {
                        Bold = true,
                    },
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE",
                        BackgroundColor = new Color {
                            Red = subTitleBackgroundColor[0],
                            Green = subTitleBackgroundColor[1],
                            Blue = subTitleBackgroundColor[2]
                        }
                    }
                ).asRequest(),
                  baseFactory.updateCells(
                    sheetId,
                    new List<object> {
                        "Based on 30"
                    },
                    3,
                    2,
                    new TextFormat {
                        Bold = true,
                    },
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE",
                        BackgroundColor = new Color {
                            Red = subTitleBackgroundColor[0],
                            Green = subTitleBackgroundColor[1],
                            Blue = subTitleBackgroundColor[2]
                        }
                    }
                ).asRequest()
            };
        }

        private IList<Request> createTimeLabelRequests(BaseRequestFactory baseFactory) {
            // add time preview colum values
            var timePreviewLabelColumnValues = new List<object>();
            var timePreviewColumnValues = new List<object>();
            for (int i = 0; i <= 30; i++) {
                if (i == 0) {
                    timePreviewLabelColumnValues.Insert(0, "now");
                    timePreviewColumnValues.Add(new Formula {
                        value = "=IFERROR(TIME(0;0;INDIRECT(\"weather_data!$B$2\"));\"\")"
                    });
                } else {
                    timePreviewLabelColumnValues.Add("in " + i + " min");
                    timePreviewColumnValues.Add(new Formula {
                        value = "=IFERROR(TIME(0;0;INDIRECT(\"weather_data!$B$2\") + " + i * 60 +");\"\")"
                    });
                }
            }

            return new List<Request> {
                baseFactory.updateColumn(
                    sheetId,
                    timePreviewLabelColumnValues,
                    0,
                    3,
                    null,
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE"
                    }
                ).asRequest(),
                baseFactory.updateColumn(
                    sheetId,
                    timePreviewColumnValues,
                    1,
                    3,
                    null,
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE",
                        NumberFormat = new NumberFormat {
                            Type = "DATE_TIME",
                            Pattern = "hh:mm"
                        }
                    }
                ).asRequest()
            };
        }

        private IList<Request> createFormulaRequests(BaseRequestFactory baseFactory) {
            // add first 10 min values based on 10
            var firstTenMinValuesBasedOnTen = new List<object>();
            for (int i = 11; i >= 2; i--) {
                firstTenMinValuesBasedOnTen.Add(
                    new Formula {
                        value = "=IFERROR(INDIRECT(\"weather_data!$H$" + i + "\");\"\")"
                    }
                );
            }

            // add first 10 min values based on 30
            var firstTenMinValuesBasedOnThirty = new List<object>();
            for (int i = 31; i >= 22; i--) {
                firstTenMinValuesBasedOnThirty.Add(
                    new Formula {
                        value = "=IFERROR(INDIRECT(\"weather_data!$I$" + i + "\");\"\")"
                    }
                );
            }

            // add last 20 min values based on 30
            var lastTwentyMinValuesBasedOnThirty = new List<object>();
            for (int i = 21; i >= 2; i--) {
                lastTwentyMinValuesBasedOnThirty.Add(
                    new Formula {
                        value = "=IFERROR(INDIRECT(\"weather_data!$I$" + i + "\");\"\")" 
                    }
                );
            }

            return new List<Request> {
                baseFactory.updateCells(
                    sheetId,
                    new List<object> {
                        new Formula {
                            value = "=IFERROR(INDIRECT(\"weather_data!$C$2\");\"\")"
                        }
                    },
                    2,
                    3,
                    null,
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE"
                    }
                ).asRequest(),
                baseFactory.updateColumn(
                    sheetId,
                    firstTenMinValuesBasedOnTen,
                    2,
                    4,
                    null,
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE"
                    }
                ).asRequest(),
                baseFactory.updateColumn(
                    sheetId,
                    firstTenMinValuesBasedOnThirty,
                    3,
                    4,
                    null,
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE"
                    }
                ).asRequest(),
                baseFactory.updateColumn(
                    sheetId,
                    lastTwentyMinValuesBasedOnThirty,
                    2,
                    14,
                    null,
                    new CellFormat {
                        HorizontalAlignment = "CENTER",
                        VerticalAlignment = "MIDDLE"
                    }
                ).asRequest()
            };
        }

        private IList<Request> createMergeRequests(BaseRequestFactory baseFactory) {
            return new List<Request> {
                baseFactory.mergeRange( // Main title
                    sheetId,
                    MergeType.HORIZONTAL,
                    0,
                    1,
                    4,
                    2
                ).asRequest(),
                baseFactory.mergeRange( // Current weather
                    sheetId,
                    MergeType.HORIZONTAL,
                    0,
                    2,
                    2,
                    3
                ).asRequest(),
                baseFactory.mergeRange( // 30 min weather
                    sheetId,
                    MergeType.HORIZONTAL,
                    2,
                    3,
                    4,
                    4
                ).asRequest(),
                baseFactory.mergeRange( // 30 min weather
                    sheetId,
                    MergeType.HORIZONTAL,
                    2,
                    14,
                    4,
                    34
                ).asRequest()
            };
        }

        private IList<Request> generateResizeRequests(BaseRequestFactory baseFactory) {
            return new List<Request> {
                baseFactory.setDimensionSize(
                    sheetId,
                    100,
                    Dimension.COLUMNS,
                    0,
                    4
                ).asRequest(),
                baseFactory.setDimensionSize(
                    sheetId,
                    40,
                    Dimension.ROWS,
                    0,
                    3
                ).asRequest()
            };
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
