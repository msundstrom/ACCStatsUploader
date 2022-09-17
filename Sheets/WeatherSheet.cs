using System.Collections.Generic;

namespace ACCStatsUploader {
    public class WeatherSheet : Sheet {

        public WeatherSheet() {
            sheetTitle = Sheet.SHEET_NAMES.WEATHER;
            columnTitles = new List<object>{
                "Ingame clock",
                "10 min forecast",
                "30 min forecast",
                "Track state"
            };
        }
    }
}
