using System;
using System.Collections.Generic;
using System.Text;

namespace ACCStatsUploader {
    public class PitInEvent {
        public float pitInSessionTime;
        public float pitBoxInSessionTime;
        public float inLap;
        public string driverName;
        public int tyreSet;

        public PitInEvent(Graphics graphicsInfo, StaticInfo staticInfo) {
            pitInSessionTime = graphicsInfo.sessionTimeLeft;
            inLap = graphicsInfo.completedLaps + 1;
            driverName = staticInfo.PlayerName + " " + staticInfo.PlayerSurname;
            tyreSet = graphicsInfo.currentTyreSet;
        }

        public void setInBox(Graphics graphicsInfo) {
            pitBoxInSessionTime = graphicsInfo.sessionTimeLeft;
        }
    }
}
