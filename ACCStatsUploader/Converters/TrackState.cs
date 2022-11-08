using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.Converters {
    public struct TrackStateConverter {
        public static string toString(int value) {
            switch (value) {
                case 0:
                    return "Green";
                case 1:
                    return "Fast";
                case 2:
                    return "Optimum";
                case 3:
                    return "Greasy";
                case 4:
                    return "Damp";
                case 5:
                    return "Wet";
                case 6:
                    return "Flooded";
                default:
                    return "Error!";
            }
        }
    }
}
