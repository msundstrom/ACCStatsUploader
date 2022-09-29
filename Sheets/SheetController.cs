﻿using Google.Apis.Sheets.v4.Data;
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

        public async Task<bool> init(SheetsAPIController gsController) {
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
                    return false;
                }
                lapSheet.sheetId = (int)lapSheetId!;

                // clear sheet + insert new cols
                await gsController.setupSheet(lapSheet.sheetId, lapSheet.columnTitles.Count);


                await gsController.appendRow(
                    lapSheet.sheetId,
                    lapSheet.columnTitles,
                    new TextFormat() {
                        Bold = true,
                        FontSize = 16
                    },
                    true
                );
            }

            if (pitstopSheet == null) {
                pitstopSheet = new PitstopSheet();
                var pitstopSheetId = await gsController.createSheet(pitstopSheet);
                if (pitstopSheetId == null) {
                    MessageBox.Show("Creating PitstopSheet failed!");
                    return false;
                }
                pitstopSheet.sheetId = (int)pitstopSheetId;

                // clear sheet + insert new cols
                await gsController.setupSheet(pitstopSheet.sheetId, pitstopSheet.columnTitles.Count);

                await gsController.appendRow(
                    pitstopSheet.sheetId,
                    pitstopSheet.columnTitles,
                    new TextFormat() {
                        Bold = true,
                        FontSize = 16
                    },
                    true
                );
            }

            if (weatherSheet == null) {
                weatherSheet = new WeatherSheet();
                var weatherSheetId = await gsController.createSheet(weatherSheet);
                if (weatherSheetId == null) {
                    MessageBox.Show("Creating WeatherSheet failed!");
                    return false;
                }
                weatherSheet.sheetId = (int)weatherSheetId;

                // clear sheet + insert new cols
                await gsController.setupSheet(weatherSheet.sheetId, weatherSheet.columnTitles.Count);

                await gsController.appendRow(
                    weatherSheet.sheetId,
                    weatherSheet.columnTitles,
                    new TextFormat() {
                        Bold = true,
                        FontSize = 16
                    },
                    true
                );
            }

            return true;
        }

        public async Task insertLapInfo(LapInfo lapInfo) {
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
                    "PitIn",
                    pitInEvent.inLap,
                    "",
                    pitInEvent.pitInSessionTime,
                    "",
                    pitInEvent.pitBoxInSessionTime,
                    "",
                    pitInEvent.driverName,
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
                    "PitOut",
                    "",
                    pitOutEvent.outLap,
                    "",
                    pitOutEvent.pitOutSessionTime,
                    "",
                    pitOutEvent.pitBoxOutSessionTime,
                    pitOutEvent.driverName,
                    pitOutEvent.tyreSet
                }
            );
        }

        public async Task insertWeatherEvent(WeatherUpdateEvent weatherEvent) {
            await gsController.appendRow(
                weatherSheet.sheetId,
                new List<object> {
                    weatherEvent.inGameClock,
                    weatherEvent.currentWeather,
                    weatherEvent.tenMinuteForecast,
                    weatherEvent.thirtyMinuteForecast,
                    weatherEvent.trackState
                }
            );
        }
    }
}
