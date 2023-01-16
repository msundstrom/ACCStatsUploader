using ACCStatsUploader.GoogleAPI;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACCStatsUploader.CarDumpJson;

namespace ACCStatsUploader {
    public class TyreSetsSheet: Sheet {
        public string sheetTitle {
            get {
                return SHEET_NAMES.TYRE_SETS;
            }
        }

        private IList<object> topRowTitles {
            get {
                return new List<object>{
                    "FL",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "FR",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "RL",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "RR",
                    "",
                    "",
                    "",
                    "",
                    "",
                };
            }
        }

        private IList<object> secondRowTitles {
            get {
                return new List<object>{
                    "Tyre set Nr",
                    "Wear",
                    "Blister",
                    "Grain",
                    "Marbles",
                    "Flatspots",
                    "Is critical?",
                    "Wear",
                    "Blister",
                    "Grain",
                    "Marbles",
                    "Flatspots",
                    "Is critical?",
                    "Wear",
                    "Blister",
                    "Grain",
                    "Marbles",
                    "Flatspots",
                    "Is critical?",
                    "Wear",
                    "Blister",
                    "Grain",
                    "Marbles",
                    "Flatspots",
                    "Is critical?",
                };
            }
        }

        public int sheetId { get; set; }
        public bool hidden {
            get {
                return false;
            }
        }

        private SheetsAPIController gsController { get; set; }

        public TyreSetsSheet(SheetsAPIController gsController) {
            this.gsController = gsController;
        }

        public async Task create() {
            await this.createSheet(gsController);
            await setup();
        }

        private async Task setup() {
            var setupRequest = gsController.createSheetRequest();

            setupRequest.addRequests(this.clearSheet());
            setupRequest.addRequest(this.addEmptyColumns(24));

            // setup top column titles
            setupRequest.addRequest(
                this.updateCells(
                    new CellRange { startRow = 0, startCol = 1 },
                    topRowTitles.Select(title => {
                        return new Cell {
                            value = title,
                            format = CellFormats.centered
                        };
                    }).ToList()
                )
            );

            setupRequest.addRequest(
                this.mergeRange(
                    new CellRange { startRow = 0, startCol = 1, endRow = 1, endCol = 7 },
                    MergeType.HORIZONTAL
                )
            );
            setupRequest.addRequest(
                this.mergeRange(
                    new CellRange { startRow = 0, startCol = 7, endRow = 1, endCol = 13 },
                    MergeType.HORIZONTAL
                )
            );
            setupRequest.addRequest(
                this.mergeRange(
                    new CellRange { startRow = 0, startCol = 13, endRow = 1, endCol = 19 },
                    MergeType.HORIZONTAL
                )
            );
            setupRequest.addRequest(
                this.mergeRange(
                    new CellRange { startRow = 0, startCol = 19, endRow = 1, endCol = 25 },
                    MergeType.HORIZONTAL
                )
            );

            // setup second row titles
            setupRequest.addRequest(this.appendRow(
                secondRowTitles.Select(title => {
                    return new Cell {
                        value = title
                    };
                }).ToList()
            ));

            await setupRequest.execute();
        }

        public async void addTyreSetData(CarDumpJson.TyreSet tyreSet) {
            double getWear(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                if (tyreIndex == 0 && tyreIndex == 2) {
                    return tyreSet.wearStatus[tyreIndex].treadMM[2];
                } else {
                    return tyreSet.wearStatus[tyreIndex].treadMM[0];
                }
            }

            double getBlister(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].blister;
            }

            double getGrain(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].grain;
            }

            double getMarbles(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].marblesLevel;
            }

            double getFlatspots(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].flatSpot;
            }

            bool getIsCriticalState(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].isCriticalState;
            }

            var lfWear = getWear(tyreSet, 0);
            var lfBlister = getBlister(tyreSet, 0);
            var lfGrain = getGrain(tyreSet, 0);
            var lfMarbles = getMarbles(tyreSet, 0);
            var lfFlatspots = getFlatspots(tyreSet, 0);
            var lfIsCritical = getIsCriticalState(tyreSet, 0);

            var rfWear = getWear(tyreSet, 1);
            var rfBlister = getBlister(tyreSet, 1);
            var rfGrain = getGrain(tyreSet, 1);
            var rfMarbles = getMarbles(tyreSet, 1);
            var rfFlatspots = getFlatspots(tyreSet, 1);
            var rfIsCritical = getIsCriticalState(tyreSet, 1);

            var lrWear = getWear(tyreSet, 2);
            var lrBlister = getBlister(tyreSet, 2);
            var lrGrain = getGrain(tyreSet, 2);
            var lrMarbles = getMarbles(tyreSet, 2);
            var lrFlatspots = getFlatspots(tyreSet, 2);
            var lrIsCritical = getIsCriticalState(tyreSet, 2);

            var rrWear = getWear(tyreSet, 3);
            var rrBlister = getBlister(tyreSet, 3);
            var rrGrain = getGrain(tyreSet, 3);
            var rrMarbles = getMarbles(tyreSet, 3);
            var rrFlatspots = getFlatspots(tyreSet, 3);
            var rrIsCritical = getIsCriticalState(tyreSet, 3);

            var row =  new List<Cell> {
                    new Cell { value = tyreSet.tyreSet + 1 },
                    new Cell { value = lfWear },
                    new Cell { value = lfBlister },
                    new Cell { value = lfGrain },
                    new Cell { value = lfMarbles },
                    new Cell { value = lfFlatspots },
                    new Cell { value = lfIsCritical },
                    new Cell { value = rfWear },
                    new Cell { value = rfBlister },
                    new Cell { value = lfGrain },
                    new Cell { value = rfMarbles },
                    new Cell { value = rfFlatspots },
                    new Cell { value = rfIsCritical },
                    new Cell { value = lrWear },
                    new Cell { value = lrBlister },
                    new Cell { value = lfGrain },
                    new Cell { value = lrMarbles },
                    new Cell { value = lrFlatspots },
                    new Cell { value = lrIsCritical },
                    new Cell { value = rrWear },
                    new Cell { value = rrBlister },
                    new Cell { value = lfGrain },
                    new Cell { value = rrMarbles },
                    new Cell { value = rrFlatspots },
                    new Cell { value = rrIsCritical },
                };

            var updateRequest = gsController.createSheetRequest();
            updateRequest.addRequest(
                this.appendRow(row)
            );

            await updateRequest.execute();
        }

        public async void updateTyreSetData(List<CarDumpJson.TyreSet> tyreSets) {

            double getWear(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                if (tyreIndex == 0 && tyreIndex == 2) {
                    return tyreSet.wearStatus[tyreIndex].treadMM[2];
                } else {
                    return tyreSet.wearStatus[tyreIndex].treadMM[0];
                }
            }

            double getBlister(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].blister;
            }

            double getGrain(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].grain;
            }

            double getMarbles(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].marblesLevel;
            }

            double getFlatspots(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].flatSpot;
            }

            bool getIsCriticalState(CarDumpJson.TyreSet tyreSet, int tyreIndex) {
                return tyreSet.wearStatus[tyreIndex].isCriticalState;
            }

            var rows = tyreSets.Select(tyreSet => {
                var lfWear = getWear(tyreSet, 0);
                var lfBlister = getBlister(tyreSet, 0);
                var lfGrain = getGrain(tyreSet, 0);
                var lfMarbles = getMarbles(tyreSet, 0);
                var lfFlatspots = getFlatspots(tyreSet, 0);
                var lfIsCritical = getIsCriticalState(tyreSet, 0);

                var rfWear = getWear(tyreSet, 1);
                var rfBlister = getBlister(tyreSet, 1);
                var rfGrain = getGrain(tyreSet, 1);
                var rfMarbles = getMarbles(tyreSet, 1);
                var rfFlatspots = getFlatspots(tyreSet, 1);
                var rfIsCritical = getIsCriticalState(tyreSet, 1);

                var lrWear = getWear(tyreSet, 2);
                var lrBlister = getBlister(tyreSet, 2);
                var lrGrain = getGrain(tyreSet, 2);
                var lrMarbles = getMarbles(tyreSet, 2);
                var lrFlatspots = getFlatspots(tyreSet, 2);
                var lrIsCritical = getIsCriticalState(tyreSet, 2);

                var rrWear = getWear(tyreSet, 3);
                var rrBlister = getBlister(tyreSet, 3);
                var rrGrain = getGrain(tyreSet, 3);
                var rrMarbles = getMarbles(tyreSet, 3);
                var rrFlatspots = getFlatspots(tyreSet, 3);
                var rrIsCritical = getIsCriticalState(tyreSet, 3);

                return new List<Cell> {
                    new Cell { value = tyreSet.tyreSet + 1 },
                    new Cell { value = lfWear }, 
                    new Cell { value = lfBlister },
                    new Cell { value = lfGrain },
                    new Cell { value = lfMarbles },
                    new Cell { value = lfFlatspots },
                    new Cell { value = lfIsCritical },
                    new Cell { value = rfWear },
                    new Cell { value = rfBlister },
                    new Cell { value = lfGrain },
                    new Cell { value = rfMarbles },
                    new Cell { value = rfFlatspots },
                    new Cell { value = rfIsCritical },
                    new Cell { value = lrWear },
                    new Cell { value = lrBlister },
                    new Cell { value = lfGrain },
                    new Cell { value = lrMarbles },
                    new Cell { value = lrFlatspots },
                    new Cell { value = lrIsCritical },
                    new Cell { value = rrWear },
                    new Cell { value = rrBlister },
                    new Cell { value = lfGrain },
                    new Cell { value = rrMarbles },
                    new Cell { value = rrFlatspots },
                    new Cell { value = rrIsCritical },
                };

            }).ToList();

            var updateRequest = gsController.createSheetRequest();
            updateRequest.addRequest(
                this.updateCells(
                    new CellRange { startRow = 3, startCol = 0 },
                    rows
                )
            );

            await updateRequest.execute();
        }
    }
}
