using ACCStatsUploader.Converters;
using ACCStatsUploader.GoogleAPI;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACCStatsUploader {
    public class StintOverviewSheet : Sheet {
        public StintOverviewSheet() {
            sheetTitle = SHEET_NAMES.STINT_OVERVIEW;
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
                30
            ).asRequest());
            request.addRequest(baseFactory.appendDimension(
                sheetId,
                Dimension.ROWS,
                42
            ).asRequest());

            // set sizes
            request.addRequests(createResizeRequest(baseFactory));

            // set title
            request.addRequests(createTitleRequets(baseFactory));

            var tyrePressureFormat = new NumberFormat {
                Type = "NUMBER",
                Pattern = "##.#"
            };
            var temperatureFormat = new NumberFormat {
                Type = "NUMBER",
                Pattern = "###"
            };
            var lapTimeFormat = new NumberFormat {
                Type = "DATE_TIME",
                Pattern = "mm:ss.000"
            };

            // add row
            for (var i = 2; i <= 41; i++) {

                var actualRow = i + 1;

                var stintLabel = "" + (i - 1);
                var outLap = "=INDIRECT(\"stint_matrix!B\"&ROW()-1)";
                var inLap = "=INDIRECT(\"stint_matrix!C\"&ROW()-1)";
                var stintLaps = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");$C" + actualRow + "-$B" + actualRow + ";\"\")";
                var stintDuration = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");(INDIRECT(\"stint_matrix!E\"&ROW()-1)-INDIRECT(\"stint_matrix!D\"&ROW()-1))/86400;\"\")";
                var driver = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDEX(UNIQUE(INDIRECT(\"lap_data!C1\"&INDIRECT(\"B\"&ROW())+1&\":C\"&INDIRECT(\"C\"&ROW())+1));1);\"\")";
                var avgPace = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!G\"&MATCH(B" + actualRow + ";lap_data!$B$1:$B;0)&\":G\"&MATCH(C" + actualRow + ";lap_data!$B$1:$B;0)))/86400000;\"\")";
                var avgPaceExcludingInAndOut = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!G\"&MATCH(B" + actualRow + ";lap_data!$B$1:$B;0)&\":G\"&MATCH(C" + actualRow + ";lap_data!$B$1:$B;0)-1))/86400000;\"\")";
                var bestLap = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");MIN(INDIRECT(\"lap_data!G\"&MATCH(B" + actualRow + ";lap_data!$B$1:$B;0)&\":G\"&MATCH(C" + actualRow + ";lap_data!$B$1:$B;0)-1))/86400000;\"\")";
                var worstLap = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");MAX(INDIRECT(\"lap_data!G\"&MATCH(B" + actualRow + ";lap_data!$B$1:$B;0)+1&\":G\"&MATCH(C" + actualRow + ";lap_data!$B$1:$B;0)-1))/86400000;\"\")";
                var outLapPace = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");IF(B" + actualRow + "=1;\"-\";INDIRECT(\"lap_data!G\"&MATCH($B" + actualRow + ";lap_data!$B$1:$B;0))/86400000);\"\")";
                var inLapPace = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");IF(INDIRECT(\"lap_data!M\"&MATCH($C" + actualRow + ";lap_data!B:B;0))=0;\"-\";INDIRECT(\"lap_data!G\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0))/86400000);\"\")";
                var fuelUse = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!L\"&MATCH(B" + actualRow + ";lap_data!$B$1:$B;0)+1&\":L\"&MATCH(C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var tyreSet = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDEX(UNIQUE(INDIRECT(\"pit_stop_data!K\"&MATCH(C" + actualRow + ";pit_stop_data!$C$1:$C));1);\"\");\"\")";

                var startTyrePressuresFL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDIRECT(\"pit_stop_data!L\"&MATCH($B" + actualRow + ";pit_stop_data!$D$1:$D;0));\"\")";
                var startTyrePressuresFR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDIRECT(\"pit_stop_data!M\"&MATCH($B" + actualRow + ";pit_stop_data!$D$1:$D;0));\"\")";
                var startTyrePressuresRL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDIRECT(\"pit_stop_data!N\"&MATCH($B" + actualRow + ";pit_stop_data!$D$1:$D;0));\"\")";
                var startTyrePressuresRR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDIRECT(\"pit_stop_data!O\"&MATCH($B" + actualRow + ";pit_stop_data!$D$1:$D;0));\"\")";

                var avgTyrePressuresFL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";
                var avgTyrePressuresFR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AR\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";
                var avgTyrePressuresRL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AS\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";
                var avgTyrePressuresRR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AT\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";

                var avgTyreTempFL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AM\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";
                var avgTyreTempFR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AN\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";
                var avgTyreTempRL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AO\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";
                var avgTyreTempRR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AP\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";

                var avgBrakeTempFL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AA\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";
                var avgBrakeTempFR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AB\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";
                var avgBrakeTempRL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AC\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";
                var avgBrakeTempRR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AD\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)-10));\"\")";

                request.addRequest(baseFactory.updateCell(sheetId, stintLabel, 0, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    TextFormat = new TextFormat {
                        Bold = true,
                        FontSize = 11
                    }
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = outLap }, 1, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = inLap }, 2, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = stintLaps }, 3, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = stintDuration }, 4, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = lapTimeFormat
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = driver }, 5, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgPace }, 6, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = lapTimeFormat
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgPaceExcludingInAndOut }, 7, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = lapTimeFormat
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = bestLap }, 8, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = lapTimeFormat
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = worstLap }, 9, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = lapTimeFormat
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = outLapPace }, 10, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = lapTimeFormat
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = inLapPace }, 11, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = lapTimeFormat
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = fuelUse }, 12, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = tyreSet }, 13, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                }).asRequest());
                if (i != 2) {
                    request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = startTyrePressuresFL }, 14, i, null, new CellFormat {
                        VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                        HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                        NumberFormat = tyrePressureFormat,
                    }).asRequest());
                    request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = startTyrePressuresFR }, 15, i, null, new CellFormat {
                        VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                        HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                        NumberFormat = tyrePressureFormat,
                    }).asRequest());
                    request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = startTyrePressuresRL }, 16, i, null, new CellFormat {
                        VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                        HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                        NumberFormat = tyrePressureFormat,
                    }).asRequest());
                    request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = startTyrePressuresRR }, 17, i, null, new CellFormat {
                        VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                        HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                        NumberFormat = tyrePressureFormat,
                    }).asRequest());
                }

                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgTyrePressuresFL }, 18, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = tyrePressureFormat,
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgTyrePressuresFR }, 19, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = tyrePressureFormat,
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgTyrePressuresRL }, 20, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = tyrePressureFormat,
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgTyrePressuresRR }, 21, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = tyrePressureFormat,
                }).asRequest());

                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgTyreTempFL }, 22, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = temperatureFormat,
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgTyreTempFR }, 23, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = temperatureFormat,
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgTyreTempRL }, 24, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = temperatureFormat,
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgTyreTempRR }, 25, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = temperatureFormat,
                }).asRequest());

                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgBrakeTempFL }, 26, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = temperatureFormat,
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgBrakeTempFR }, 27, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = temperatureFormat,
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgBrakeTempRL }, 28, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = temperatureFormat,
                }).asRequest());
                request.addRequest(baseFactory.updateCell(sheetId, new Formula { value = avgBrakeTempRR }, 29, i, null, new CellFormat {
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER),
                    NumberFormat = temperatureFormat,
                }).asRequest());
            }

            // set borders
            request.addRequests(createBorderRequests(baseFactory));

            // merge top row
            request.addRequests(createMergeRequest(baseFactory));

            // freeze two first row and first col
            request.addRequest(baseFactory.freezeRows(
                sheetId,
                2
            ).asRequest());

            request.addRequest(baseFactory.freezeColumns(
                sheetId,
                1
            ).asRequest());

            // set time formats
            request.addRequests(setTimeFormats(baseFactory));

            

            await request.execute();
        }

        private IList<Request> setTimeFormats(BaseRequestFactory baseFactory) {
            return new List<Request> {
                baseFactory.updateColumn(
                    sheetId,
                    new List<object>(),
                    4,
                    2,
                    null,
                    new CellFormat {
                        NumberFormat = new NumberFormat {
                            Type = "DATE_TIME",
                            Pattern = "mm:ss.000"
                        }
                    },
                    "userEnteredFormat.numberFormat"
                ).asRequest()
            };
        }

        private IList<Request> createTitleRequets(BaseRequestFactory baseFactory) {
            var firstTitleRow = new List<object> {
                "Start tyre pressures",
                "Avg PSI (last 10 lap)",
                "Avg tyre ºC (last 10)",
                "Avg brake ºC (last 10)"
            };
            var secondTitleRow = new List<object> {
                    "Stint number",
                    "Outlap",
                    "Inlap",
                    "Stint laps",
                    "Stint duration",
                    "Driver",
                    "Avg pace",
                    "Avg pace (excl. in&out)",
                    "Best lap",
                    "Worst lap",
                    "Outlap",
                    "Inlap",
                    "Fuel per lap",
                    "Tyre set",
                    "FL",
                    "FR",
                    "RL",
                    "RR",
                    "FL",
                    "FR",
                    "RL",
                    "RR",
                    "FL",
                    "FR",
                    "RL",
                    "RR",
                    "FL",
                    "FR",
                    "RL",
                    "RR"
                };
            
            return new List<Request> {
                baseFactory.updateCell(sheetId, firstTitleRow[0], 14, 0, null, new CellFormat {
                    TextFormat = new TextFormat {
                        Bold = true,
                        FontSize = 11
                    },
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER)
                }).asRequest(),
                baseFactory.updateCell(sheetId, firstTitleRow[1], 18, 0, null, new CellFormat {
                    TextFormat = new TextFormat {
                        Bold = true,
                        FontSize = 11
                    },
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER)
                }).asRequest(),
                baseFactory.updateCell(sheetId, firstTitleRow[2], 22, 0, null, new CellFormat {
                    TextFormat = new TextFormat {
                        Bold = true,
                        FontSize = 11
                    },
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER)
                }).asRequest(),
                baseFactory.updateCell(sheetId, firstTitleRow[3], 26, 0, null, new CellFormat {
                    TextFormat = new TextFormat {
                        Bold = true,
                        FontSize = 11
                    },
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER)
                }).asRequest(),
                baseFactory.updateCells(sheetId, secondTitleRow, 0, 1, null, new CellFormat {
                    TextFormat = new TextFormat {
                        Bold = true,
                        FontSize = 11
                    },
                    VerticalAlignment = AlignmentConverter.Vertical.toString(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.Horizontal.toString(CellHorizontalAlignment.CENTER)
                }).asRequest()
            };
        }

        private IList<Request> createBorderRequests(BaseRequestFactory baseRequestFactory) {
            return new List<Request> {
                baseRequestFactory.setRightBorder(sheetId, new Border {
                    Style = "SOLID_MEDIUM",
                }, 0, null, 13, 14).asRequest(),
                baseRequestFactory.setRightBorder(sheetId, new Border {
                    Style = "SOLID_MEDIUM"
                }, 0, null, 17, 18).asRequest(),
                baseRequestFactory.setRightBorder(sheetId, new Border {
                    Style = "SOLID_MEDIUM"
                }, 0, null, 21, 22).asRequest(),
                baseRequestFactory.setRightBorder(sheetId, new Border {
                    Style = "SOLID_MEDIUM"
                }, 0, null, 25, 26).asRequest(),
            };
        }

        private IList<Request> createMergeRequest(BaseRequestFactory baseFactory) {
            return new List<Request> {
                baseFactory.mergeRange(
                    sheetId,
                    MergeType.HORIZONTAL,
                    1,
                    0,
                    14,
                    1
                ).asRequest(),
                baseFactory.mergeRange(
                    sheetId,
                    MergeType.HORIZONTAL,
                    14,
                    0,
                    18,
                    1
                ).asRequest(),
                baseFactory.mergeRange(
                    sheetId,
                    MergeType.HORIZONTAL,
                    18,
                    0,
                    22,
                    1
                ).asRequest(),
                baseFactory.mergeRange(
                    sheetId,
                    MergeType.HORIZONTAL,
                    22,
                    0,
                    26,
                    1
                ).asRequest(),
                baseFactory.mergeRange(
                    sheetId,
                    MergeType.HORIZONTAL,
                    26,
                    0,
                    30,
                    1
                ).asRequest()
            };
        }

        private IList<Request> createResizeRequest(BaseRequestFactory baseFactory) {
            return new List<Request> {
                baseFactory.setDimensionSize(
                    sheetId,
                    100,
                    Dimension.COLUMNS,
                    0
                ).asRequest(),
                baseFactory.setDimensionSize(
                    sheetId,
                    40,
                    Dimension.COLUMNS,
                    14
                ).asRequest(),
                baseFactory.setDimensionSize(
                    sheetId,
                    30,
                    Dimension.ROWS,
                    0
                ).asRequest(),
                baseFactory.setDimensionSize(
                    sheetId,
                    175,
                    Dimension.COLUMNS,
                    5,
                    6
                ).asRequest(),
                baseFactory.setDimensionSize(
                    sheetId,
                    175,
                    Dimension.COLUMNS,
                    7,
                    8
                ).asRequest(),
            };
        }
    }
}
