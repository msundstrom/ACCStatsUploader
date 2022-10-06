using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    public struct Time {
        public int hours = 0;
        public int minutes = 0;
        public int seconds = 0;

        public string hourMinuteString {
            get {
                return hours.ToString().PadLeft(2, '0') + ":" + minutes.ToString().PadLeft(2, '0');
            }
        }

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
}
