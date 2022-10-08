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
    }
}
