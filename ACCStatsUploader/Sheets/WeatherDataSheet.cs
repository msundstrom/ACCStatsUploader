﻿using ACCStatsUploader.GoogleAPI;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    using IRequestList = IList<Request>;
    using RequestList = List<Request>;
    using ICells = IList<Cell>;
    using Cells = List<Cell>;
    using ColorConverter = Converters.ColorConverter;
    public class WeatherDataSheet : Sheet {

        public string sheetTitle {
            get {
                return SHEET_NAMES.WEATHER;
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
                    "Ingame clock",
                    "Ingame clock (raw value)",
                    "Current weather",
                    "Air temp",
                    "Track temp",
                    "Wind",
                    "Track state",
                    "10 min forecast",
                    "30 min forecast",
                };
            }
        }

        private SheetsAPIController gsController { get; set; }

        public WeatherDataSheet(SheetsAPIController gsController) {
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

        public async Task insertWeatherEvent(WeatherUpdateEvent weatherEvent) {
            var insertEventRequest = gsController.createSheetRequest();

            var cells = new Cells {
                new Cell { value = weatherEvent.inGameClock.hourMinuteString },
                new Cell { value = (weatherEvent.inGameClock.hours * 60 * 60) + (weatherEvent.inGameClock.minutes * 60) },
                new Cell { value = weatherEvent.currentWeather },
                new Cell { value = weatherEvent.airTemp },
                new Cell { value = weatherEvent.trackTemp },
                new Cell { value = weatherEvent.windSpeed },
                new Cell { value = weatherEvent.trackState },
                new Cell { value = weatherEvent.tenMinuteForecast },
                new Cell { value = weatherEvent.thirtyMinuteForecast }
            };

            insertEventRequest.addRequests(this.insertRow(cells, new CellRange { startRow = 1, endRow = 2 }));

            await insertEventRequest.execute();
        }
    }
}
