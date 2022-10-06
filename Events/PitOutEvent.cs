using ACCStatsUploader.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACCStatsUploader {
    public class PitOutEvent {
        public string sessionType;
        public float pitOutClockTime;
        public float pitBoxOutClockTime;
        public float outLap;
        public string driverName;
        public int tyreSet;
        public Wheels initialTyrePressures;

        public PitOutEvent(Graphics graphicsInfo, Physics physicsInfo) {
            sessionType = SessionTypeConverter.toString(graphicsInfo.session);
            pitBoxOutClockTime = graphicsInfo.Clock;
            tyreSet = graphicsInfo.currentTyreSet;

            initialTyrePressures = new Wheels(
                physicsInfo.wheelsPressure[0],
                physicsInfo.wheelsPressure[1],
                physicsInfo.wheelsPressure[2],
                physicsInfo.wheelsPressure[3]
            );
        }

        public void setPitOut(Graphics graphicsInfo, StaticInfo staticInfo) {
            outLap = graphicsInfo.completedLaps + 1;
            pitOutClockTime = graphicsInfo.Clock;
            driverName = staticInfo.PlayerName + " " + staticInfo.PlayerSurname;
        }
    }
}
