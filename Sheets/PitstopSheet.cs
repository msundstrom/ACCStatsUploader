using ACCStatsUploader.GoogleAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    using ICells = IList<Cell>;
    using Cells = List<Cell>;
    public class PitstopSheet: Sheet {
        public string sheetTitle {
            get {
                return SHEET_NAMES.PITSTOP;
            }
        }

        private IList<object> columnTitles {
            get {
                return new List<object>{
                    "Session",
                    "Type",
                    "Inlap",
                    "Outlap",
                    "Pit in session time left",
                    "Pit out session time left",
                    "Pitbox in session time left",
                    "Pitbox out session time left",
                    "Driver",
                    "Drive time left",
                    "Tyre set",
                    "Initial PSI FL",
                    "Initial PSI FR",
                    "Initial PSI RL",
                    "Initial PSI RR"
                };
            }
        }
        public int sheetId { get; set; }
        public bool hidden {
            get {
                return true;
            }
        }
        private SheetsAPIController gsController { get; set; }

        public PitstopSheet(SheetsAPIController gsController) {
            this.gsController = gsController;
        }

        public async Task create() {
            await this.createSheet(gsController);
            await setup();
        }

        public async Task setup() {
            var setupRequest = gsController.createSheetRequest();

            setupRequest.addRequests(this.clearSheet());
            setupRequest.addRequest(this.addEmptyColumns(columnTitles.Count - 1));
            setupRequest.addRequest(this.appendRow(
                columnTitles.Select(title => {
                    return new Cell {
                        value = title
                    };
                }).ToList()
            ));

            await setupRequest.execute();
        }

        public async Task insertPitInEvent(PitInEvent pitInEvent) {
            var pitEventRequest = gsController.createSheetRequest();

            pitEventRequest.addRequest(this.appendRow(new Cells {
                new Cell { value = pitInEvent.sessionType },
                new Cell { value = pitInEvent.type },
                new Cell { value = pitInEvent.inLap },
                new Cell { value = "" },
                new Cell { value = pitInEvent.pitInClockTime },
                new Cell { value = "" },
                new Cell { value = pitInEvent.pitBoxInClockTime },
                new Cell { value = "" },
                new Cell { value = pitInEvent.driverName },
                new Cell { value = pitInEvent.driveTimeLeft },
                new Cell { value = pitInEvent.tyreSet }
            }));

            await pitEventRequest.execute();
        }

        public async Task insertPitOutEvent(PitOutEvent pitOutEvent) {
            var pitEventRequest = gsController.createSheetRequest();

            pitEventRequest.addRequest(this.appendRow(new Cells {
                new Cell { value = pitOutEvent.sessionType },
                new Cell { value = pitOutEvent.type },
                new Cell { value = "" },
                new Cell { value = pitOutEvent.outLap },
                new Cell { value = "" },
                new Cell { value = pitOutEvent.pitOutClockTime },
                new Cell { value = "" },
                new Cell { value = pitOutEvent.pitBoxOutClockTime },
                new Cell { value = pitOutEvent.driverName },
                new Cell { value = "" },
                new Cell { value = pitOutEvent.tyreSet },
                new Cell { value = pitOutEvent.initialTyrePressures.fl },
                new Cell { value = pitOutEvent.initialTyrePressures.fr },
                new Cell { value = pitOutEvent.initialTyrePressures.rl },
                new Cell { value = pitOutEvent.initialTyrePressures.rr },
            }));

            await pitEventRequest.execute();
        }
    }
}
