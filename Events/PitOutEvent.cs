using System;
using System.Collections.Generic;
using System.Text;

namespace ACCStatsUploader {
    public class PitOutEvent {
        public float pitOutSessionTime;
        public float pitBoxOutSessionTime;
        public float outLap;
        public string driverName;
        public int tyreSet;

        public PitOutEvent(Graphics graphicsInfo) {
            pitBoxOutSessionTime = graphicsInfo.sessionTimeLeft;
            tyreSet = graphicsInfo.currentTyreSet;
        }

        public void setPitOut(Graphics graphicsInfo, StaticInfo staticInfo) {
            outLap = graphicsInfo.completedLaps + 1;
            pitOutSessionTime = graphicsInfo.sessionTimeLeft;
            driverName = staticInfo.PlayerName + " " + staticInfo.PlayerName;
        }
    }
}
