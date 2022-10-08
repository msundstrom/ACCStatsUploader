using ACCStatsUploader.GoogleAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACCStatsUploader {
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
    }
}
