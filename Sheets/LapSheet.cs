using ACCStatsUploader.GoogleAPI;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACCStatsUploader {
    using ICells = IList<Cell>;
    using Cells = List<Cell>;

    public class LapSheet : Sheet {
        public string sheetTitle {
            get {
                return SHEET_NAMES.LAP;
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
                return new List<object>{
                    "Session Type",
                    "Lap",
                    "Driver",
                    "Sector 1",
                    "Sector 2",
                    "Sector 3",
                    "Lap time",
                    "Valid lap?",
                    "Out lap?",
                    "In lap?",
                    "Fuel level (end of lap)",
                    "Fuel consumption",
                    "Session time left",
                    "Position overall",
                    "Car count",
                    "In-game Clock",
                    "Air temp",
                    "Track temp",
                    "Damage front",
                    "Damage right",
                    "Damage rear",
                    "Damage left",
                    "Brake pad level FL",
                    "Brake pad level FR",
                    "Brake pad level RL",
                    "Brake pad level RR",
                    "Brake avg temp FL",
                    "Brake avg temp FR",
                    "Brake avg temp RL",
                    "Brake avg temp RR",
                    "Brake max temp FL",
                    "Brake max temp FR",
                    "Brake max temp RL",
                    "Brake max temp RR",
                    "Avg tyre temp FL",
                    "Avg tyre temp FR",
                    "Avg tyre temp RL",
                    "Avg tyre temp RR",
                    "Max tyre temp FL",
                    "Max tyre temp FR",
                    "Max tyre temp RL",
                    "Max tyre temp RR",
                    "Avg tyre pressure FL",
                    "Avg tyre pressure FR",
                    "Avg tyre pressure RL",
                    "Avg tyre pressure RR",
                    "Max tyre pressure FL",
                    "Max tyre pressure FR",
                    "Max tyre pressure RL",
                    "Max tyre pressure RR",
                };
            }
        }

        private SheetsAPIController gsController { get; set; }

        public LapSheet(SheetsAPIController gsController) {
            this.gsController = gsController;
        }

        public async Task create() {
            await this.createSheet(gsController);
            await setup();
        }

        private async Task setup() {
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

        public async Task insertLap(LapInfo lapInfo) {
            var insertLapRequest = gsController.createSheetRequest();

            insertLapRequest.addRequest(this.appendRow(new Cells {                      
                new Cell { value = lapInfo.sessionType },
                new Cell { value = lapInfo.lapNumber },
                new Cell { value = lapInfo.driverName },
                new Cell { value = lapInfo.timingInfo.sectorTimes[0] },
                new Cell { value = lapInfo.timingInfo.sectorTimes[1] },
                new Cell { value = lapInfo.timingInfo.sectorTimes[2] },
                new Cell { value = lapInfo.timingInfo.lastLaptime },
                new Cell { value = lapInfo.timingInfo.isValid ? "Yes" : "No" },
                new Cell { value = lapInfo.isOutLap ? "Yes" : "No" },
                new Cell { value = lapInfo.isInLap ? "Yes" : "No" },
                new Cell { value = lapInfo.fuelInfo.lapEnd },
                new Cell { value = lapInfo.fuelInfo.fuelUsedDuringLap },
                new Cell { value = lapInfo.sessionTimeLeft },
                new Cell { value = lapInfo.position },
                new Cell { value = lapInfo.carCount },
                new Cell { value = lapInfo.gameClock },
                new Cell { value = lapInfo.airTemp },
                new Cell { value = lapInfo.trackTemp },
                new Cell { value = lapInfo.damageInfo.carDamage.front },
                new Cell { value = lapInfo.damageInfo.carDamage.right },
                new Cell { value = lapInfo.damageInfo.carDamage.rear },
                new Cell { value = lapInfo.damageInfo.carDamage.left },
                new Cell { value = lapInfo.brakeInfo.brakePads.fl },
                new Cell { value = lapInfo.brakeInfo.brakePads.fr },
                new Cell { value = lapInfo.brakeInfo.brakePads.rl },
                new Cell { value = lapInfo.brakeInfo.brakePads.rr },
                new Cell { value = lapInfo.brakeInfo.averageTemps().fl },
                new Cell { value = lapInfo.brakeInfo.averageTemps().fr },
                new Cell { value = lapInfo.brakeInfo.averageTemps().rl },
                new Cell { value = lapInfo.brakeInfo.averageTemps().rr },
                new Cell { value = lapInfo.brakeInfo.maxTemps().fl },
                new Cell { value = lapInfo.brakeInfo.maxTemps().fr },
                new Cell { value = lapInfo.brakeInfo.maxTemps().rl },
                new Cell { value = lapInfo.brakeInfo.maxTemps().rr },
                new Cell { value = lapInfo.tyreInfo.averageTemps().fl },
                new Cell { value = lapInfo.tyreInfo.averageTemps().fr },
                new Cell { value = lapInfo.tyreInfo.averageTemps().rl },
                new Cell { value = lapInfo.tyreInfo.averageTemps().rr },
                new Cell { value = lapInfo.tyreInfo.maxTemps().fl },
                new Cell { value = lapInfo.tyreInfo.maxTemps().fr },
                new Cell { value = lapInfo.tyreInfo.maxTemps().rl },
                new Cell { value = lapInfo.tyreInfo.maxTemps().rr },
                new Cell { value = lapInfo.tyreInfo.averagePressures().fl },
                new Cell { value = lapInfo.tyreInfo.averagePressures().fr },
                new Cell { value = lapInfo.tyreInfo.averagePressures().rl },
                new Cell { value = lapInfo.tyreInfo.averagePressures().rr },
                new Cell { value = lapInfo.tyreInfo.maxPressures().fl },
                new Cell { value = lapInfo.tyreInfo.maxPressures().fr },
                new Cell { value = lapInfo.tyreInfo.maxPressures().rl },
                new Cell { value = lapInfo.tyreInfo.maxPressures().rr },
            }));

            await insertLapRequest.execute();
        }
    }
}
