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
            await lapSheet.insertLap(lapInfo);
        }

        public async Task insertPitInEvent(PitInEvent pitInEvent) {
            await pitstopSheet.insertPitInEvent(pitInEvent);
        }

        public async Task insertPitOutEvent(PitOutEvent pitOutEvent) {
            await pitstopSheet.insertPitOutEvent(pitOutEvent);
        }

        public async Task insertWeatherEvent(WeatherUpdateEvent weatherEvent) {
            await weatherSheet.insertWeatherevent(weatherEvent);
        }
    }
}
