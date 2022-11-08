using ACCStatsUploader.Converters;

namespace ACCStatsUploader {
    public class WeatherUpdateEvent {
        public Time inGameClock;
        public int currentWeatherValue;
        public float airTemp;
        public float trackTemp;
        public float windSpeed;
        public int tenMinuteForecastValue;
        public int thirtyMinuteForecastValue;
        public int trackStateValue;

        public string currentWeather {
            get {
                return weatherStringFromValue(currentWeatherValue);
            }
        }

        public string tenMinuteForecast {
            get {
                return weatherStringFromValue(tenMinuteForecastValue);
            }
        }

        public string thirtyMinuteForecast {
            get {
                return weatherStringFromValue(thirtyMinuteForecastValue);
            }
        }

        public string trackState {
            get {
                return TrackStateConverter.toString(trackStateValue);
            }
        }

        private string weatherStringFromValue(int value) {
            return WeatherConverter.toString(value);
        }
    }
}
