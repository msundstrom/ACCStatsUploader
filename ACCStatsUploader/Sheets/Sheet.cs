using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    public class SheetCreationFailedException : Exception { }

    public static class SHEET_NAMES {
        public static String LAP = "lap_data";
        public static String PITSTOP = "pit_stop_data";
        public static String WEATHER = "weather_data";
        public static String FORECAST = "Weather forecast";
        public static String STINT_MATRIX = "stint_matrix";
        public static String STINT_OVERVIEW = "Stint overview";
        public static String DRIVER_OVERVIEW = "Driver overview";
    }

    public interface Sheet {
        string sheetTitle { get; }
        public int sheetId { get; set; }
        bool hidden { get; }

        public Task create();
    }
}
