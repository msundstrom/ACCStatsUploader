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
    using ICells = IList<Cell>;
    using Cells = List<Cell>;
    public class StintOverviewSheet : Sheet {
        public string sheetTitle {
            get {
                return SHEET_NAMES.STINT_OVERVIEW;
            }
        }
        public int sheetId { get; set; }
        public bool hidden {
            get {
                return false;
            }
        }

        private SheetsAPIController gsController { get; set; }

        private CellFormat centeredFormat {
            get {
                return new CellFormat {
                    VerticalAlignment = AlignmentConverter.vertical(CellVerticalAlignment.MIDDLE),
                    HorizontalAlignment = AlignmentConverter.horizontal(CellHorizontalAlignment.CENTER)
                };
            }
        }

        private IList<object> secondRowTitles {
            get {
                return new List<object> {
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
            }
        }

        public StintOverviewSheet(SheetsAPIController gsController) {
            this.gsController = gsController;
        }

        public async Task create() {
            await this.createSheet(gsController);
            await setup();
        }

        private async Task setup() {
            var setupRequest = gsController.createSheetRequest();

            setupRequest.addRequests(this.clearSheet());
            setupRequest.addRequest(this.addEmptyColumns(29));
            setupRequest.addRequest(this.addEmptyRows(41));

            setupRequest.addRequest(this.resize(Dimension.ROWS, new CellRange { startRow = 0 }, 30));
            setupRequest.addRequest(this.resize(Dimension.COLUMNS, new CellRange { startCol = 0 }, 100));
            setupRequest.addRequest(this.resize(Dimension.COLUMNS, new CellRange { startCol = 14 }, 40));
            setupRequest.addRequest(this.resize(Dimension.COLUMNS, new CellRange { startCol = 5, endCol = 6 }, 175));
            setupRequest.addRequest(this.resize(Dimension.COLUMNS, new CellRange { startCol = 7, endCol = 8 }, 175));

            // setup top row titles
            setupRequest.addRequest(this.updateCell(
                new CellRange { startCol = 14, startRow = 0 },
                new Cell { value = "Start tyre pressures", format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) }
            ));

            setupRequest.addRequest(this.updateCell(
                new CellRange { startCol = 18, startRow = 0 },
                new Cell { value = "Avg PSI (last 10 lap)", format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) }
            ));

            setupRequest.addRequest(this.updateCell(
                new CellRange { startCol = 22, startRow = 0 },
                new Cell { value = "Avg tyre ºC (last 10)", format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) }
            ));

            setupRequest.addRequest(this.updateCell(
                new CellRange { startCol = 26, startRow = 0 },
                new Cell { value = "Avg brake ºC (last 10)", format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) }
            ));

            // setup second row titles
            setupRequest.addRequest(this.updateCells(
                new CellRange { startCol = 0, startRow = 1 },
                secondRowTitles.Select(title => {
                    return new Cell { value = title, format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) };
                }).ToList()
            ));

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
                var avgPaceExcludingInAndOut = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!G\"&MATCH(B" + actualRow + ";lap_data!$B$1:$B;0)+1&\":G\"&MATCH(C" + actualRow + ";lap_data!$B$1:$B;0)-1))/86400000;\"\")";
                var bestLap = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");MIN(INDIRECT(\"lap_data!G\"&MATCH(B" + actualRow + ";lap_data!$B$1:$B;0)&\":G\"&MATCH(C" + actualRow + ";lap_data!$B$1:$B;0)-1))/86400000;\"\")";
                var worstLap = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");MAX(INDIRECT(\"lap_data!G\"&MATCH(B" + actualRow + ";lap_data!$B$1:$B;0)+1&\":G\"&MATCH(C" + actualRow + ";lap_data!$B$1:$B;0)-1))/86400000;\"\")";
                var outLapPace = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");IF(B" + actualRow + "=1;\"-\";INDIRECT(\"lap_data!G\"&MATCH($B" + actualRow + ";lap_data!$B$1:$B;0))/86400000);\"\")";
                var inLapPace = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");IF(INDIRECT(\"lap_data!M\"&MATCH($C" + actualRow + ";lap_data!B:B;0))=0;\"-\";INDIRECT(\"lap_data!G\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0))/86400000);\"\")";
                var fuelUse = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!L\"&MATCH(B" + actualRow + ";lap_data!$B$1:$B;0)+1&\":L\"&MATCH(C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var tyreSet = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");IFNA(INDEX(UNIQUE(INDIRECT(\"pit_stop_data!K\"&MATCH(B" + actualRow + ";pit_stop_data!$D$1:$D));1);\"\");INDEX(UNIQUE(INDIRECT(\"pit_stop_data!K\"&MATCH(C" + actualRow + ";pit_stop_data!$C$1:$C));1);\"\"));\"\")";

                object startTyrePressuresFL;
                object startTyrePressuresFR;
                object startTyrePressuresRL;
                object startTyrePressuresRR;

                if (i == 2) {
                    startTyrePressuresFL = "";
                    startTyrePressuresFR = "";
                    startTyrePressuresRL = "";
                    startTyrePressuresRR = "";
                } else {
                    startTyrePressuresFL = new Formula { value = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDIRECT(\"pit_stop_data!L\"&MATCH($B" + actualRow + ";pit_stop_data!$D$1:$D;0));\"\")" };
                    startTyrePressuresFR = new Formula { value = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDIRECT(\"pit_stop_data!M\"&MATCH($B" + actualRow + ";pit_stop_data!$D$1:$D;0));\"\")" };
                    startTyrePressuresRL = new Formula { value = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDIRECT(\"pit_stop_data!N\"&MATCH($B" + actualRow + ";pit_stop_data!$D$1:$D;0));\"\")" };
                    startTyrePressuresRR = new Formula { value = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");INDIRECT(\"pit_stop_data!O\"&MATCH($B" + actualRow + ";pit_stop_data!$D$1:$D;0));\"\")" };
                }
                
                var avgTyrePressuresFL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AQ\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AQ\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var avgTyrePressuresFR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AR\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AR\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var avgTyrePressuresRL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AS\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AS\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var avgTyrePressuresRR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AT\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AT\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";

                var avgTyreTempFL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AM\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AM\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var avgTyreTempFR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AN\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AN\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var avgTyreTempRL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AO\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AO\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var avgTyreTempRR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AP\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AP\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";

                var avgBrakeTempFL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AA\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AA\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var avgBrakeTempFR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AB\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AB\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var avgBrakeTempRL = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AC\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AC\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";
                var avgBrakeTempRR = "=IF(AND($B" + actualRow + "<>\"\";$C" + actualRow + "<>\"\");AVERAGE(INDIRECT(\"lap_data!AD\"&MAX(MATCH($C" + actualRow + ";lap_data!$B$1:$B;0) - 10;2)&\":AD\"&MATCH($C" + actualRow + ";lap_data!$B$1:$B;0)));\"\")";


                var cells = new Cells {
                    new Cell {
                        value = stintLabel,
                        format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat),
                    },
                    new Cell { value = new Formula { value = outLap }, format = CellFormats.centered },
                    new Cell { value = new Formula { value = inLap }, format = CellFormats.centered },
                    new Cell { value = new Formula { value = stintLaps }, format = CellFormats.centered },
                    new Cell { value = new Formula { value = stintDuration }, format = CellFormats.centeredWithNumberFormat(NumberFormats.laptimeFormat) },
                    new Cell { value = new Formula { value = driver }, format = CellFormats.centered },
                    new Cell { value = new Formula { value = avgPace }, format = CellFormats.centeredWithNumberFormat(NumberFormats.laptimeFormat) },
                    new Cell { value = new Formula { value = avgPaceExcludingInAndOut }, format = CellFormats.centeredWithNumberFormat(NumberFormats.laptimeFormat) },
                    new Cell { value = new Formula { value = bestLap }, format = CellFormats.centeredWithNumberFormat(NumberFormats.laptimeFormat) },
                    new Cell { value = new Formula { value = worstLap }, format = CellFormats.centeredWithNumberFormat(NumberFormats.laptimeFormat) },
                    new Cell { value = new Formula { value = outLapPace }, format = CellFormats.centeredWithNumberFormat(NumberFormats.laptimeFormat) },
                    new Cell { value = new Formula { value = inLapPace }, format = CellFormats.centeredWithNumberFormat(NumberFormats.laptimeFormat) },
                    new Cell { value = new Formula { value = fuelUse }, format = CellFormats.centeredWithNumberFormat(NumberFormats.fuelFormat) },
                    new Cell { value = new Formula { value = tyreSet }, format = centeredFormat },
                    new Cell { value = startTyrePressuresFL, format = CellFormats.centeredWithNumberFormat(NumberFormats.psiFormat) },
                    new Cell { value = startTyrePressuresFR, format = CellFormats.centeredWithNumberFormat(NumberFormats.psiFormat) },
                    new Cell { value = startTyrePressuresRL, format = CellFormats.centeredWithNumberFormat(NumberFormats.psiFormat) },
                    new Cell { value = startTyrePressuresRR, format = CellFormats.centeredWithNumberFormat(NumberFormats.psiFormat) },
                    new Cell { value = new Formula { value = avgTyrePressuresFL }, format = CellFormats.centeredWithNumberFormat(NumberFormats.psiFormat) },
                    new Cell { value = new Formula { value = avgTyrePressuresFR }, format = CellFormats.centeredWithNumberFormat(NumberFormats.psiFormat) },
                    new Cell { value = new Formula { value = avgTyrePressuresRL }, format = CellFormats.centeredWithNumberFormat(NumberFormats.psiFormat) },
                    new Cell { value = new Formula { value = avgTyrePressuresRR }, format = CellFormats.centeredWithNumberFormat(NumberFormats.psiFormat) },
                    new Cell { value = new Formula { value = avgTyreTempFL }, format = CellFormats.centeredWithNumberFormat(NumberFormats.temperatureFormat) },
                    new Cell { value = new Formula { value = avgTyreTempFR }, format = CellFormats.centeredWithNumberFormat(NumberFormats.temperatureFormat) },
                    new Cell { value = new Formula { value = avgTyreTempRL }, format = CellFormats.centeredWithNumberFormat(NumberFormats.temperatureFormat) },
                    new Cell { value = new Formula { value = avgTyreTempRR }, format = CellFormats.centeredWithNumberFormat(NumberFormats.temperatureFormat) },
                    new Cell { value = new Formula { value = avgBrakeTempFL }, format = CellFormats.centeredWithNumberFormat(NumberFormats.temperatureFormat) },
                    new Cell { value = new Formula { value = avgBrakeTempFR }, format = CellFormats.centeredWithNumberFormat(NumberFormats.temperatureFormat) },
                    new Cell { value = new Formula { value = avgBrakeTempRL }, format = CellFormats.centeredWithNumberFormat(NumberFormats.temperatureFormat) },
                    new Cell { value = new Formula { value = avgBrakeTempRR }, format = CellFormats.centeredWithNumberFormat(NumberFormats.temperatureFormat) },
                };
                setupRequest.addRequest(this.updateCells(new CellRange { startCol = 0, startRow = i }, cells));
            }

            // set borders
            setupRequest.addRequest(this.setBorder(BorderStyle.SOLID_MEDIUM, BorderEdge.RIGHT, new CellRange { startRow = 0, startCol = 13, endCol = 14 }));
            setupRequest.addRequest(this.setBorder(BorderStyle.SOLID_MEDIUM, BorderEdge.RIGHT, new CellRange { startRow = 0, startCol = 17, endCol = 18 }));
            setupRequest.addRequest(this.setBorder(BorderStyle.SOLID_MEDIUM, BorderEdge.RIGHT, new CellRange { startRow = 0, startCol = 21, endCol = 22 }));
            setupRequest.addRequest(this.setBorder(BorderStyle.SOLID_MEDIUM, BorderEdge.RIGHT, new CellRange { startRow = 0, startCol = 25, endCol = 26 }));

            // merge top row
            setupRequest.addRequest(this.mergeRange(new CellRange { startCol = 1, startRow = 0, endCol = 14, endRow = 1 }, MergeType.HORIZONTAL));
            setupRequest.addRequest(this.mergeRange(new CellRange { startCol = 14, startRow = 0, endCol = 18, endRow = 1 }, MergeType.HORIZONTAL));
            setupRequest.addRequest(this.mergeRange(new CellRange { startCol = 18, startRow = 0, endCol = 22, endRow = 1 }, MergeType.HORIZONTAL));
            setupRequest.addRequest(this.mergeRange(new CellRange { startCol = 22, startRow = 0, endCol = 26, endRow = 1 }, MergeType.HORIZONTAL));
            setupRequest.addRequest(this.mergeRange(new CellRange { startCol = 26, startRow = 0, endCol = 30, endRow = 1 }, MergeType.HORIZONTAL));

            setupRequest.addRequest(this.freezeRows(2));
            setupRequest.addRequest(this.freezeColumns(1));

            await setupRequest.execute();
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
    }
}
