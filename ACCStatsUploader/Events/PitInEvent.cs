using ACCStatsUploader.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACCStatsUploader {
    public class PitInEvent {
        public string type = "PitIn";
        public string sessionType;
        public float pitInClockTime;
        public float pitBoxInClockTime;
        public float inLap;
        public string driverName;
        public int tyreSet;
        public int driveTimeLeft;

        public PitInEvent(Graphics graphicsInfo, StaticInfo staticInfo) {
            sessionType = SessionTypeConverter.toString(graphicsInfo.session);
            pitInClockTime = graphicsInfo.Clock;
            inLap = graphicsInfo.completedLaps + 1;
            driverName = staticInfo.PlayerName + " " + staticInfo.PlayerSurname;
            tyreSet = graphicsInfo.currentTyreSet;
            driveTimeLeft = graphicsInfo.driverStintTotalTimeLeft;
        }

        public void setInBox(Graphics graphicsInfo) {
            pitBoxInClockTime = graphicsInfo.Clock;
        }
    }
}
