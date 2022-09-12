using System;
using System.Collections.Generic;
using System.Text;

namespace ACCStatsUploader {
    public class TimingInfo {
        public List<int> sectorTimes = new List<int>(3);
        public int lastLaptime = 0;
        public bool isValid;

        private int lastSectorIndex = 0;

        public void update(Graphics graphicsUpdate) {
            if (lastSectorIndex < graphicsUpdate.currentSectorIndex) {
                var sectorTime = graphicsUpdate.lastSectorTime;
                
                if (graphicsUpdate.currentSectorIndex == 2 && sectorTimes.Count == 1) {
                    sectorTime -= sectorTimes[0];
                }
                sectorTimes.Add(sectorTime);
                lastSectorIndex = graphicsUpdate.currentSectorIndex;
            }
            isValid = graphicsUpdate.isValidLap == 1;
        }

        public void endLap(Graphics graphicsUpdate) {
            if (sectorTimes.Count == 2) {
                sectorTimes.Add(graphicsUpdate.iLastTime - (sectorTimes[0] + sectorTimes[1]));
            } else {
                sectorTimes.Add(6000);
            }
            
            lastLaptime = graphicsUpdate.iLastTime;
        }
    }
}
