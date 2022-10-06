using System.Collections.Generic;

namespace ACCStatsUploader {
    public class WeatherDataSheet : Sheet {

        public WeatherDataSheet() {
            sheetTitle = Sheet.SHEET_NAMES.WEATHER;
            columnTitles = new List<object>{
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
}
