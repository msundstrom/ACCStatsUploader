using System;
using System.Collections.Generic;
using System.Text;

namespace ACCStatsUploader {
    public class Sheet {
        public static class SHEET_NAMES {
            public static String LAP = "Lap info";
            public static String PITSTOP = "Pit stop info";
            public static String WEATHER = "Weather info";
        }

        public string sheetTitle;
        public IList<string> columnTitles;
        public int sheetId;
    }
}
