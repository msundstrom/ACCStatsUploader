using System;
using System.Collections.Generic;
using System.Text;

namespace ACCStatsUploader {
    public class Sheet {
        public static class SHEET_NAMES {
            public static String LAP = "lap_data";
            public static String PITSTOP = "pit_stop_data";
            public static String WEATHER = "weather_data";
            public static String FORECAST = "weather_forecast";
        }

        public string sheetTitle;
        public IList<object> columnTitles;
        public int sheetId;
        public bool hidden = false;
    }
}
