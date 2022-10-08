using ACCStatsUploader.GoogleAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    using ICells = IList<Cell>;
    using Cells = List<Cell>;
    public class DriverOverviewSheet: Sheet {
        public string sheetTitle {
            get {
                return SHEET_NAMES.DRIVER_OVERVIEW;
            }
        }
        public int sheetId { get; set; }
        public bool hidden {
            get {
                return false;
            }
        }

        private SheetsAPIController gsController { get; set; }

        public DriverOverviewSheet(SheetsAPIController sheetsAPIController) {
            this.gsController = sheetsAPIController;
        }

        public async Task create() {
            await this.createSheet(gsController);
            await setup();
        }

        private async Task setup() {
            var setupRequest = gsController.createSheetRequest();

            setupRequest.addRequests(this.clearSheet());
            setupRequest.addRequest(this.addEmptyColumns(4));
            setupRequest.addRequest(this.addEmptyRows(10));

            setupRequest.addRequest(this.resize(Dimension.ROWS, new CellRange { startRow = 0 }, 30));
            setupRequest.addRequest(this.resize(Dimension.COLUMNS, new CellRange { startCol = 0 }, 150));
            setupRequest.addRequest(this.resize(Dimension.COLUMNS, new CellRange { startCol = 1, endCol = 2 }, 150));

            setupRequest.addRequest(this.updateCells(
                new CellRange { startRow = 0, startCol = 1 },
                new Cells {
                    new Cell { value = "Name", format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) },
                    new Cell { value = "Drive time left", format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) },
                    new Cell { value = "Stint count", format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) },
                    new Cell { value = "Average pace", format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) },
                }
            ));

            for (int i = 1; i <= 10; i++) {
                setupRequest.addRequest(this.updateCells(
                    new CellRange { startRow = i, startCol = 0 },
                    new Cells {
                        new Cell { value = i, format = CellFormats.centeredWithTextFormat(TextFormats.titleFormat) },
                        new Cell {
                            value = new Formula { value = "=IFERROR(INDEX(UNIQUE(FILTER('Stint overview'!$F$3:$F; 'Stint overview'!$F$3:$F<>\"\"));ROW()-1);\"\")"},
                            format = CellFormats.centered
                        },
                        new Cell {
                            value = new Formula { value = "=IF(INDIRECT(\"$B\"&ROW())<>\"\";ARRAYFORMULA(VLOOKUP(INDIRECT(\"$B\"&ROW())&\" PitIn\";{pit_stop_data!$I:$I&\" \"&pit_stop_data!$B:$B\\pit_stop_data!$J:$J};2;false));\"\")"},
                            format = CellFormats.centered
                        },
                        new Cell {
                            value = new Formula { value = "=IF(INDIRECT(\"$B\"&ROW())<>\"\";COUNTA(FILTER('Stint overview'!$F$3:$F;'Stint overview'!$F$3:$F=INDIRECT(\"$B\"&ROW())));\"\")"},
                            format = CellFormats.centered
                        },
                        new Cell {
                            value = new Formula { value = "=IF(INDIRECT(\"$B\"&ROW())<>\"\";AVERAGE(FILTER('Stint overview'!$H$3:$H;'Stint overview'!$F$3:$F=INDIRECT(\"$B\"&ROW())));\"\")"},
                            format = CellFormats.centered
                        },
                    }
                ));
            }

            await setupRequest.execute();
        }
    }
}
