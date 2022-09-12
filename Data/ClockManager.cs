using System;

namespace ACCStatsUploader {
    public struct Time {
        public int hours = 0;
        public int minutes = 0;
        public int seconds = 0;

        public static bool operator >(Time a, Time b) {
            if (a.hours > b.hours)
                return true;
            if (a.minutes > b.minutes)
                return true;
            // lets just ignore seconds...
            //if (a.seconds > b.seconds)
            //    return true;
            return false;
        }

        public static bool operator <(Time a, Time b) {
            if (a.hours < b.hours)
                return true;
            if (a.minutes < b.minutes)
                return true;
            // lets just ignore seconds...
            //if (a.seconds < b.seconds)
            //    return true;
            return false;
        }

        public Time(float seconds) {
            hours = (int)Math.Floor(seconds / 3600);
            minutes = (int)Math.Floor((seconds % 3600) / 60);
            seconds = (int)Math.Floor((seconds % 60));
        }
    }

    public class ClockManager {

        public Time currentTime;

        public Time constructTimeFromClockValue(float clockValue) {
            return new Time(clockValue);
        }

        public void update(Graphics graphics) {
            currentTime = new Time(graphics.Clock);
        }
    }
}
