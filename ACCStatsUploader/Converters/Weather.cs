using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.Converters {
    public struct WeatherConverter {
        public static string toString(int value) {
            switch (value) {
                case 0:
                    return "Clear";
                case 1:
                    return "Drizzle";
                case 2:
                    return "Light rain";
                case 3:
                    return "Medium rain";
                case 4:
                    return "Heavy rain";
                case 5:
                    return "Thunderstorm";
                default:
                    return "Error!";
            }
        }
    }
}
