using System;
using System.Collections.Generic;

namespace ACCStatsUploader {
    public class LapInfo {
        public struct FuelInfo {
            public float lapStart;
            public float lapEnd;

            public float fuelUsedDuringLap {
                get {
                    return lapStart - lapEnd;
                }
            }

            public void endLap(Physics physicsUpdate) {
                lapEnd = physicsUpdate.fuel;
            }
        }

        public List<int> sectorTimes;
        public int lapTime;
        public float currentFuel;
        public int lapNumber;
        public bool isValidLap = true;
        public bool isInLap = false;
        public bool isOutLap = false;
        public TyreInfo tyreInfo;
        public FuelInfo fuelInfo = new FuelInfo();
        public TimingInfo timingInfo = new TimingInfo();
        public string driverName;
        public float sessionTimeLeft;
        public int position;
        public int carCount;
        public float gameClock;

        public DamageInfo damageInfo = new DamageInfo();
        public BrakeInfo brakeInfo = new BrakeInfo();

        private PitstopInfo? ongoingPitstop = null;

        public LapInfo(Graphics initialGraphicsData, Physics initialPhysicsData, StaticInfo initialStaticInfo) {
            this.sectorTimes = new List<int>();
            this.lapTime = 0;
            this.lapNumber = initialGraphicsData.completedLaps + 1;
            tyreInfo = new TyreInfo();


            lapNumber = initialGraphicsData.completedLaps + 1;
            fuelInfo.lapStart = initialPhysicsData.fuel;
           
        }

        public void update(Physics physicsUpdate) {
            tyreInfo.update(physicsUpdate);
        }

        public void update(Graphics graphicsUpdate) {
            if (graphicsUpdate.isValidLap == 0) {
                isValidLap = false;
            }

            if (graphicsUpdate.isInPitLane == 1 && ongoingPitstop == null) {
                ongoingPitstop = new PitstopInfo(graphicsUpdate);
            }

            if (ongoingPitstop != null) {
                ongoingPitstop.update(graphicsUpdate);
            }

            timingInfo.update(graphicsUpdate);
        }

        public void endLap(Graphics graphicsUpdate, Physics physicsUpdate, StaticInfo staticInfo) {
            driverName = staticInfo.PlayerName + " " + staticInfo.PlayerSurname;
            sessionTimeLeft = graphicsUpdate.sessionTimeLeft;
            position = graphicsUpdate.position;
            carCount = graphicsUpdate.activeCars;
            gameClock = graphicsUpdate.Clock;

            timingInfo.endLap(graphicsUpdate);
            fuelInfo.endLap(physicsUpdate);
            brakeInfo.endLap(physicsUpdate);
            damageInfo.endLap(physicsUpdate);
        }
    }
}