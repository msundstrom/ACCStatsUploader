using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.Converters {
    public struct SessionTypeConverter {
        public static string toString(int value) {
            switch (value) {
                case -1:
                    return "Unknown";
                case 0:
                    return "Practice";
                case 1:
                    return "Qualifying";
                case 2:
                    return "Race";
                case 3:
                    return "Hotlap";
                case 4:
                    return "TimeAttack";
                case 5:
                    return "Drift";
                case 6:
                    return "Drag";
                default:
                    return "Error!";
            }
        }

        public static string toString(ACC_SESSION_TYPE type) {
            return SessionTypeConverter.toString((int)type);
        }
    }
}
