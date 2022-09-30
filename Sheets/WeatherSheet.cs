using System.Collections.Generic;

namespace ACCStatsUploader {
    public class WeatherSheet : Sheet {

        public WeatherSheet() {
            sheetTitle = Sheet.SHEET_NAMES.WEATHER;
            columnTitles = new List<object>{
                "Ingame clock",
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
