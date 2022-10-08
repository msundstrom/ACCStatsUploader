using ACCStatsUploader.GoogleAPI;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACCStatsUploader {
    public class SheetController {
        public PitstopSheet pitstopSheet;
        public LapSheet lapSheet;
        public WeatherDataSheet weatherSheet;
        public ForecastSheet forecastSheet;
        public StintOverviewSheet stintOverviewSheet;
        public StintMatrixSheet stintMatrixSheet;
        public DriverOverviewSheet driverOverviewSheet;

        private SheetsAPIController gsController;

        private BaseRequestFactory baseRequestFactory = new BaseRequestFactory();
        private CompoundRequestFactory compoundRequestFactory = new CompoundRequestFactory();

        public async Task<bool> init(SheetsAPIController gsController) {
            this.gsController = gsController;

            var existingSheets = await gsController.getSheets();

            foreach (Google.Apis.Sheets.v4.Data.Sheet sheet in existingSheets) {
                if (sheet.Properties.Title == SHEET_NAMES.LAP) {
                    lapSheet = new LapSheet(gsController) { sheetId = (int)sheet.Properties.SheetId };
                } else if (sheet.Properties.Title == SHEET_NAMES.PITSTOP) {
                    pitstopSheet = new PitstopSheet(gsController) { sheetId = (int)sheet.Properties.SheetId};
                } else if (sheet.Properties.Title == SHEET_NAMES.WEATHER) {
                    weatherSheet = new WeatherDataSheet(gsController) { sheetId = (int)sheet.Properties.SheetId};
                } else if (sheet.Properties.Title == SHEET_NAMES.FORECAST) {
                    forecastSheet = new ForecastSheet(gsController) { sheetId = (int)sheet.Properties.SheetId };
                } else if (sheet.Properties.Title == SHEET_NAMES.STINT_OVERVIEW) {
                    stintOverviewSheet = new StintOverviewSheet(gsController) { sheetId = (int)sheet.Properties.SheetId };
                } else if (sheet.Properties.Title == SHEET_NAMES.STINT_MATRIX) {
                    stintMatrixSheet = new StintMatrixSheet(gsController) { sheetId = (int)sheet.Properties.SheetId };
                } else if (sheet.Properties.Title == SHEET_NAMES.DRIVER_OVERVIEW) {
                    driverOverviewSheet = new DriverOverviewSheet(gsController) { sheetId = (int)sheet.Properties.SheetId };
                }
            }

            if (lapSheet == null) {
                lapSheet = new LapSheet(gsController);
                await lapSheet.create();
            }

            if (pitstopSheet == null) {
                pitstopSheet = new PitstopSheet(gsController);
                await pitstopSheet.create();
            }

            if (weatherSheet == null) {
                weatherSheet = new WeatherDataSheet(gsController);
                await weatherSheet.create();
            }

            if (forecastSheet == null) {
                forecastSheet = new ForecastSheet(gsController);
                await forecastSheet.create();
            }

            if (stintMatrixSheet == null) {
                stintMatrixSheet = new StintMatrixSheet(gsController);
                await stintMatrixSheet.create();
            }

            if (stintOverviewSheet == null) {
                stintOverviewSheet = new StintOverviewSheet(gsController);
                await stintOverviewSheet.create();
            }

            if (driverOverviewSheet == null) {
                driverOverviewSheet = new DriverOverviewSheet(gsController);
                await driverOverviewSheet.create();
            }

            return true;
        }

        private async Task<int?> createSheet(Sheet sheet) {
            var weatherSheetId = await gsController.createSheet(sheet);
            if (weatherSheetId == null) {
                MessageBox.Show("Creating " + sheet.sheetTitle + " failed!");
            }

            return weatherSheetId;
        }

        public async Task insertLapInfo(LapInfo lapInfo) {
            if (lapInfo.timingInfo.sectorTimes.Count != 3) {
                MessageBox.Show("Wrong number of sectors!");
            }

            await gsController.appendRow(
                lapSheet.sheetId,
                new List<object> {
                    lapInfo.sessionType,
                    lapInfo.lapNumber,
                    lapInfo.driverName,
                    lapInfo.timingInfo.sectorTimes[0],
                    lapInfo.timingInfo.sectorTimes[1],
                    lapInfo.timingInfo.sectorTimes[2],
                    lapInfo.timingInfo.lastLaptime,
                    lapInfo.timingInfo.isValid ? "Yes" : "No",
                    lapInfo.isOutLap ? "Yes" : "No",
                    lapInfo.isInLap ? "Yes" : "No",
                    lapInfo.fuelInfo.lapEnd,
                    lapInfo.fuelInfo.fuelUsedDuringLap,
                    lapInfo.sessionTimeLeft,
                    lapInfo.position,
                    lapInfo.carCount,
                    lapInfo.gameClock,
                    lapInfo.airTemp,
                    lapInfo.trackTemp,
                    lapInfo.damageInfo.carDamage.front,
                    lapInfo.damageInfo.carDamage.right,
                    lapInfo.damageInfo.carDamage.rear,
                    lapInfo.damageInfo.carDamage.left,
                    lapInfo.brakeInfo.brakePads.fl,
                    lapInfo.brakeInfo.brakePads.fr,
                    lapInfo.brakeInfo.brakePads.rl,
                    lapInfo.brakeInfo.brakePads.rr,
                    lapInfo.brakeInfo.averageTemps().fl,
                    lapInfo.brakeInfo.averageTemps().fr,
                    lapInfo.brakeInfo.averageTemps().rl,
                    lapInfo.brakeInfo.averageTemps().rr,
                    lapInfo.brakeInfo.maxTemps().fl,
                    lapInfo.brakeInfo.maxTemps().fr,
                    lapInfo.brakeInfo.maxTemps().rl,
                    lapInfo.brakeInfo.maxTemps().rr,
                    lapInfo.tyreInfo.averageTemps().fl,
                    lapInfo.tyreInfo.averageTemps().fr,
                    lapInfo.tyreInfo.averageTemps().rl,
                    lapInfo.tyreInfo.averageTemps().rr,
                    lapInfo.tyreInfo.maxTemps().fl,
                    lapInfo.tyreInfo.maxTemps().fr,
                    lapInfo.tyreInfo.maxTemps().rl,
                    lapInfo.tyreInfo.maxTemps().rr,
                    lapInfo.tyreInfo.averagePressures().fl,
                    lapInfo.tyreInfo.averagePressures().fr,
                    lapInfo.tyreInfo.averagePressures().rl,
                    lapInfo.tyreInfo.averagePressures().rr,
                    lapInfo.tyreInfo.maxPressures().fl,
                    lapInfo.tyreInfo.maxPressures().fr,
                    lapInfo.tyreInfo.maxPressures().rl,
                    lapInfo.tyreInfo.maxPressures().rr
                },
                null,
                true
            );
        }

        public async Task insertPitInEvent(PitInEvent pitInEvent) {
            await gsController.appendRow(
                pitstopSheet.sheetId,
                new List<object> {
                    pitInEvent.sessionType,
                    "PitIn",
                    pitInEvent.inLap,
                    "",
                    pitInEvent.pitInClockTime,
                    "",
                    pitInEvent.pitBoxInClockTime,
                    "",
                    pitInEvent.driverName,
                    pitInEvent.driveTimeLeft,
                    pitInEvent.tyreSet
                },
                null,
                true
            );
        }

        public async Task insertPitOutEvent(PitOutEvent pitOutEvent) {
            await gsController.appendRow(
                pitstopSheet.sheetId,
                new List<object> {
                    pitOutEvent.sessionType,
                    "PitOut",
                    "",
                    pitOutEvent.outLap,
                    "",
                    pitOutEvent.pitOutClockTime,
                    "",
                    pitOutEvent.pitBoxOutClockTime,
                    pitOutEvent.driverName,
                    "",
                    pitOutEvent.tyreSet,
                    pitOutEvent.initialTyrePressures.fl,
                    pitOutEvent.initialTyrePressures.fr,
                    pitOutEvent.initialTyrePressures.rl,
                    pitOutEvent.initialTyrePressures.rr
                }
            );
        }

        public async Task insertWeatherEvent(WeatherUpdateEvent weatherEvent) {
            var sheetRequest = gsController.createSheetRequest();
            sheetRequest.addRequests(
                compoundRequestFactory.insertRow(
                weatherSheet.sheetId,
                1,
                new List<object> {
                    weatherEvent.inGameClock.hourMinuteString,
                    (weatherEvent.inGameClock.hours * 60 * 60) + (weatherEvent.inGameClock.minutes * 60),
                    weatherEvent.currentWeather,
                    weatherEvent.airTemp,
                    weatherEvent.trackTemp,
                    weatherEvent.windSpeed,
                    weatherEvent.trackState,
                    weatherEvent.tenMinuteForecast,
                    weatherEvent.thirtyMinuteForecast
                }
            ));

            await sheetRequest.execute();
        }
    }
}
