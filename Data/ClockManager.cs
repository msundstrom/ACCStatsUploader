using System;

namespace ACCStatsUploader {
    
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
