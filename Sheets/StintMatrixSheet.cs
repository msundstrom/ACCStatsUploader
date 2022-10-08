using ACCStatsUploader.GoogleAPI;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    public class StintMatrixSheet : Sheet {
        public StintMatrixSheet() {
            sheetTitle = Sheet.SHEET_NAMES.STINT_MATRIX;
            columnTitles = new List<object>();
            //hidden = true;
        }

        public async Task setup(SheetsAPIController gsController) {
            var baseFactory = new BaseRequestFactory();
            var compoundFactory = new CompoundRequestFactory();

            var request = gsController.createSheetRequest();

            request.addRequests(compoundFactory.clearSheet(sheetId));
            request.addRequest(baseFactory.appendDimension(
                sheetId,
                Dimension.COLUMNS,
                5
            ).asRequest());

            request.addRequest(
                baseFactory.updateCells(
                    sheetId,
                    new List<object> {
                        "",
                        "Start lap",
                        "End lap",
                        "Start time",
                        "End time"
                    },
                    0,
                    0
                ).asRequest()
            );

            for (int i = 1; i <= 40; i++) {
                object inLap = new Formula {
                    value = "=IFERROR(INDEX(FILTER(INDIRECT(\"pit_stop_data!D:D\");INDIRECT(\"pit_stop_data!B:B\") = \"PitOut\");$A" + (i + 1) + "-1);\"\")"
                };
                if (i == 1) {
                    inLap = 1;
                }

                var outLap = new Formula {
                    value = "=IF(INDIRECT(\"B\"&ROW())<>\"\";IFERROR(INDEX(FILTER(INDIRECT(\"pit_stop_data!C:C\");INDIRECT(\"pit_stop_data!B:B\") = \"PitIn\");$A" + (i + 1) + ");IFNA(INDEX(FILTER(INDIRECT(\"lap_data!B:B\");INDIRECT(\"lap_data!M:M\") = 0);1)));\"\")"
                };

                var startTime = new Formula {
                    value = "=IF(AND($B" + (i + 1) + "<>\"\";$C" + (i + 1) + "<>\"\");INDIRECT(\"lap_data!P\"&MATCH($B" + (i + 1) + ";lap_data!$B$1:$B;0));\"\")"
                };
                var endTime = new Formula {
                    value = "=IF(AND($B" + (i + 1) + "<>\"\";$C" + (i + 1) + "<>\"\");INDIRECT(\"lap_data!P\"&MATCH($C" + (i + 1) + ";lap_data!$B$1:$B;0));\"\")"
                };

                request.addRequest(
                    baseFactory.addRow(
                        sheetId,
                        new List<object> {
                            "" + i,
                            inLap,
                            outLap,
                            startTime,
                            endTime
                        }
                    ).asRequest()
                );

                await request.execute();
            }
        }
    }
}
