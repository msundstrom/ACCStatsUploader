using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACCStatsUploader {
    public class SheetController {
        public PitstopSheet pitstopSheet;
        public LapSheet lapSheet;
        public WeatherSheet weatherSheet;

        private SheetsAPIController gsController;

        public async Task init(SheetsAPIController gsController) {
            this.gsController = gsController;

            var existingSheets = await gsController.getSheets();

            foreach (Google.Apis.Sheets.v4.Data.Sheet sheet in existingSheets) {
                if (sheet.Properties.Title == Sheet.SHEET_NAMES.LAP) {
                    lapSheet = new LapSheet() { sheetId = (int)sheet.Properties.SheetId };
                } else if (sheet.Properties.Title == Sheet.SHEET_NAMES.PITSTOP) {
                    pitstopSheet = new PitstopSheet() { sheetId = (int)sheet.Properties.SheetId };
                } else if (sheet.Properties.Title == Sheet.SHEET_NAMES.WEATHER) {
                    weatherSheet = new WeatherSheet() { sheetId = (int)sheet.Properties.SheetId }; 
                }
            }

            if (lapSheet == null) {
                lapSheet = new LapSheet();
                var lapSheetId = await gsController.createSheet(lapSheet);
                if (lapSheetId == null) {
                    MessageBox.Show("Creating LapSheet failed!");
                }
                lapSheet.sheetId = (int)lapSheetId!;

                // if we have too many columns, we need to add some empty columns
                var defaultColumnCount = 26;

                if (defaultColumnCount < lapSheet.columnTitles.Count) {
                    var columnsToAdd = lapSheet.columnTitles.Count - defaultColumnCount;
                    await gsController.insertEmptyColumns(
                        lapSheet.sheetId,
                        columnsToAdd
                    );
                }

                await gsController.appendRow(
                    lapSheet.sheetId,
                    lapSheet.columnTitles,
                    new TextFormat() {
                        Bold = true,
                        FontSize = 16
                    }
                );
            }

            if (pitstopSheet == null) {
                pitstopSheet = new PitstopSheet();
                var pitstopSheetId = await gsController.createSheet(pitstopSheet);
                if (pitstopSheetId == null) {
                    MessageBox.Show("Creating PitstopSheet failed!");
                }
                pitstopSheet.sheetId = (int)pitstopSheetId;

                // if we have too many columns, we need to add some empty columns
                var defaultColumnCount = 26;

                if (defaultColumnCount < lapSheet.columnTitles.Count) {
                    var columnsToAdd = lapSheet.columnTitles.Count - defaultColumnCount;
                    await gsController.insertEmptyColumns(
                        lapSheet.sheetId,
                        columnsToAdd
                    );
                }

                await gsController.appendRow(
                    pitstopSheet.sheetId,
                    pitstopSheet.columnTitles,
                    new TextFormat() {
                        Bold = true,
                        FontSize = 16
                    }
                );
            }

            if (weatherSheet == null) {
                weatherSheet = new WeatherSheet();
                var weatherSheetId = await gsController.createSheet(weatherSheet);
                if (weatherSheetId == null) {
                    MessageBox.Show("Creating PitstopSheet failed!");
                }
                weatherSheet.sheetId = (int)weatherSheetId;

                // if we have too many columns, we need to add some empty columns
                var defaultColumnCount = 26;

                if (defaultColumnCount < weatherSheet.columnTitles.Count) {
                    var columnsToAdd = weatherSheet.columnTitles.Count - defaultColumnCount;
                    await gsController.insertEmptyColumns(
                        weatherSheet.sheetId,
                        columnsToAdd
                    );
                }

                await gsController.appendRow(
                    weatherSheet.sheetId,
                    weatherSheet.columnTitles,
                    new TextFormat() {
                        Bold = true,
                        FontSize = 16
                    }
                );
            }
        }

        public async Task insertLapInfo(LapInfo lapInfo) {
            await gsController.appendRow(
                lapSheet.sheetId,
                new List<string> {
                    lapInfo.lapNumber.ToString(),
                    lapInfo.driverName,
                    lapInfo.timingInfo.sectorTimes[0].ToString(),
                    lapInfo.timingInfo.sectorTimes[1].ToString(),
                    lapInfo.timingInfo.sectorTimes[2].ToString(),
                    lapInfo.timingInfo.lastLaptime.ToString(),
                    lapInfo.timingInfo.isValid ? "Yes" : "No",
                    lapInfo.isOutLap ? "Yes" : "No",
                    lapInfo.isInLap ? "Yes" : "No",
                    lapInfo.fuelInfo.lapEnd.ToString(),
                    lapInfo.fuelInfo.fuelUsedDuringLap.ToString(),
                    lapInfo.sessionTimeLeft.ToString(),
                    lapInfo.position.ToString(),
                    lapInfo.carCount.ToString(),
                    lapInfo.gameClock.ToString(),
                    lapInfo.damageInfo.carDamage.front.ToString(),
                    lapInfo.damageInfo.carDamage.right.ToString(),
                    lapInfo.damageInfo.carDamage.rear.ToString(),
                    lapInfo.damageInfo.carDamage.left.ToString(),
                    lapInfo.brakeInfo.brakePads.fl.ToString(),
                    lapInfo.brakeInfo.brakePads.fr.ToString(),
                    lapInfo.brakeInfo.brakePads.rl.ToString(),
                    lapInfo.brakeInfo.brakePads.rr.ToString(),
                    lapInfo.tyreInfo.averageTemps().fl.ToString(),
                    lapInfo.tyreInfo.averageTemps().fr.ToString(),
                    lapInfo.tyreInfo.averageTemps().rl.ToString(),
                    lapInfo.tyreInfo.averageTemps().rr.ToString(),
                    lapInfo.tyreInfo.maxTemps().fl.ToString(),
                    lapInfo.tyreInfo.maxTemps().fr.ToString(),
                    lapInfo.tyreInfo.maxTemps().rl.ToString(),
                    lapInfo.tyreInfo.maxTemps().rr.ToString(),
                    lapInfo.tyreInfo.averagePressures().fl.ToString(),
                    lapInfo.tyreInfo.averagePressures().fr.ToString(),
                    lapInfo.tyreInfo.averagePressures().rl.ToString(),
                    lapInfo.tyreInfo.averagePressures().rr.ToString(),
                    lapInfo.tyreInfo.maxPressures().fl.ToString(),
                    lapInfo.tyreInfo.maxPressures().fr.ToString(),
                    lapInfo.tyreInfo.maxPressures().rl.ToString(),
                    lapInfo.tyreInfo.maxPressures().rr.ToString()
                }
            );
        }

        public async Task insertPitInEvent(PitInEvent pitInEvent) {
            await gsController.appendRow(
                pitstopSheet.sheetId,
                new List<string> {
                    "PitIn",
                    pitInEvent.inLap.ToString(),
                    "",
                    pitInEvent.pitInSessionTime.ToString(),
                    "",
                    pitInEvent.pitBoxInSessionTime.ToString(),
                    "",
                    pitInEvent.driverName
                }
            );
        }

        public async Task insertPitOutEvent(PitOutEvent pitOutEvent) {
            await gsController.appendRow(
                pitstopSheet.sheetId,
                new List<string> {
                    "PitOut",
                    "",
                    pitOutEvent.outLap.ToString(),
                    "",
                    pitOutEvent.pitOutSessionTime.ToString(),
                    "",
                    pitOutEvent.pitBoxOutSessionTime.ToString(),
                    pitOutEvent.driverName
                }
            );
        }

        public async Task insertWeatherEvent(WeatherUpdateEvent weatherEvent) {
            await gsController.appendRow(
                weatherSheet.sheetId,
                new List<string> {
                    weatherEvent.inGameClock.ToString(),
                    weatherEvent.tenMinuteForecast.ToString(),
                    weatherEvent.thirtyMinuteForecast.ToString()
                }
            );
        }
    }
}
