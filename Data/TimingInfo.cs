using System;
using System.Collections.Generic;
using System.Printing.IndexedProperties;
using System.Text;

namespace ACCStatsUploader {
    public class TimingInfo {
        public List<int> sectorTimes = new List<int>(3);
        public int lastLaptime = 0;
        public bool isValid;

        private int _lastSectorIndex = 0;
        private int lastSectorIndex {
            get {
                return _lastSectorIndex;
            }
            set {
                System.Diagnostics.Debug.WriteLine("setting lastSectorIndex to " + value);
                _lastSectorIndex = value;
            }
        }

        public bool isFirstLap = false;

        public void update(Graphics graphicsUpdate) {
            if (isFirstLap && graphicsUpdate.currentSectorIndex < 0) {
                return;
            }

            if (lastSectorIndex < graphicsUpdate.currentSectorIndex) {
                System.Diagnostics.Debug.WriteLine("Adding sector " + graphicsUpdate.currentSectorIndex + "...");
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
                System.Diagnostics.Debug.WriteLine("Adding sector 3...");
                sectorTimes.Add(graphicsUpdate.iLastTime - (sectorTimes[0] + sectorTimes[1]));
            } else {
                sectorTimes.Add(-1);
                sectorTimes.Add(-1);
                sectorTimes.Add(-1);
            }
            
            lastLaptime = graphicsUpdate.iLastTime;
        }
    }
}
