using ACCStatsUploader.GoogleAPI;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    using ICells = IList<Cell>;
    using Cells = List<Cell>;


    public class StintMatrixSheet : Sheet {
        public string sheetTitle {
            get {
                return SHEET_NAMES.STINT_MATRIX;
            }
        }
        public int sheetId { get; set; }
        public bool hidden {
            get {
                return true;
            }
        }

        private IList<object> columnTitles {
            get {
                return new List<object> {
                    "",
                    "Start lap",
                    "End lap",
                    "Start time",
                    "End time"
                };
            }
        }

        private SheetsAPIController gsController { get; set; }

        public StintMatrixSheet(SheetsAPIController gsController) {
            this.gsController = gsController;
        }

        public async Task create() {
            await this.createSheet(gsController);
            await setup();
        }

        public async Task setup() {
            var setupRequest = gsController.createSheetRequest();

            setupRequest.addRequests(this.clearSheet());
            setupRequest.addRequest(this.addEmptyColumns(4));
            setupRequest.addRequest(this.appendRow(
                columnTitles.Select(title => {
                    return new Cell { value = title };
                }).ToList())
            );

            for (int i = 1; i <= 40; i++) {
                object inLap = new Formula {
                    value = "=IFERROR(INDEX(FILTER(INDIRECT(\"pit_stop_data!D:D\");INDIRECT(\"pit_stop_data!B:B\") = \"PitOut\";INDIRECT(\"pit_stop_data!A:A\") = \"Race\");$A" + (i + 1) + "-1);\"\")"
                };

                var outLap = new Formula {
                    value = "=IF(INDIRECT(\"B\"&ROW())<>\"\";IFERROR(INDEX(FILTER(INDIRECT(\"pit_stop_data!C:C\");INDIRECT(\"pit_stop_data!B:B\") = \"PitIn\";INDIRECT(\"pit_stop_data!A:A\") = \"Race\");$A" + (i + 1) + ");IFNA(INDEX(FILTER(INDIRECT(\"lap_data!B:B\");INDIRECT(\"lap_data!M:M\") = 0);1)));\"\")"
                };

                var startTime = new Formula {
                    value = "=IF(AND($B" + (i + 1) + "<>\"\";$C" + (i + 1) + "<>\"\");INDIRECT(\"lap_data!P\"&MATCH($B" + (i + 1) + ";lap_data!$B$1:$B;0));\"\")"
                };

                var endTime = new Formula {
                    value = "=IF(AND($B" + (i + 1) + "<>\"\";$C" + (i + 1) + "<>\"\");IFNA(INDIRECT(\"pit_stop_data!E\"&MATCH($C2; pit_stop_data!$C$1:$C;0));INDIRECT(\"lap_data!P\"&MATCH($C" + (i + 1) +"; lap_data!$B$1:$B;0)));\"\")"
                };

                // special case for the first stint
                if (i == 1) {
                    inLap = 1;
                    startTime = new Formula {
                        value = "=IF(AND($B2<>\"\";$C2<>\"\");INDIRECT(\"lap_data!P\"&MATCH($B2;lap_data!$B$1:$B;0)) - INDIRECT(\"lap_data!G\"&MATCH($B2;lap_data!$B$1:$B;0)) / 1000;\"\")"
                    };
                }

                setupRequest.addRequest(this.appendRow(new Cells {
                    new Cell { value = i },
                    new Cell { value = inLap },
                    new Cell { value = outLap },
                    new Cell { value = startTime },
                    new Cell { value = endTime }
                }));

                
            }

            await setupRequest.execute();
        }
    }
}
