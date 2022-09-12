using System;
using System.Collections.Generic;
using System.Text;

namespace ACCStatsUploader {
    public class PitstopInfo {
        public float pitInSessionTime;
        public float pitOutSessionTime;
        public float pitlaneTime {
            get {
                return pitInSessionTime - pitOutSessionTime;
            }
        }

        private float pitstopStartSessionTime;
        private float pitstopEndSessionTime;
        private bool pitstopActive;
        public float pitstopTime {
            get {
                return pitstopStartSessionTime - pitstopEndSessionTime;
            }
        }

        public int inLap;
        public int outLap {
            get {
                return inLap + 1;
            }
        }

        public PitstopInfo(Graphics graphicsData) {
            pitInSessionTime = graphicsData.sessionTimeLeft;
        }

        public void update(Graphics graphicsData) {
            if (graphicsData.isInPit == 1 && !pitstopActive) {
                pitstopActive = true;
                pitstopStartSessionTime = graphicsData.sessionTimeLeft;
            }

            if (graphicsData.isInPit == 0 && pitstopActive) {
                pitstopActive = false;
                pitstopEndSessionTime = graphicsData.sessionTimeLeft;
            }

            if (graphicsData.isInPitLane == 0) {
                pitOutSessionTime = graphicsData.sessionTimeLeft;
            }
        }
    }
}
