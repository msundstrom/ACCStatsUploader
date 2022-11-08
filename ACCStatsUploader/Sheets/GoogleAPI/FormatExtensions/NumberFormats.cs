using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.GoogleAPI {
    public static class NumberFormats {

        public static NumberFormat laptimeFormat {
            get {
                return new NumberFormat { Type = "DATE_TIME", Pattern = "mm:ss.000" };
            }
        }

        public static NumberFormat clock {
            get {
                return new NumberFormat { Type = "DATE_TIME", Pattern = "hh:mm" };
            }
        }

        public static NumberFormat temperatureFormat {
            get {
                return new NumberFormat { Type = "NUMBER", Pattern = "###" };
            }
        }

        public static NumberFormat psiFormat {
            get {
                return new NumberFormat { Type = "NUMBER", Pattern = "##.0" };
            }
        }

        public static NumberFormat fuelFormat {
            get {
                return new NumberFormat { Type = "NUMBER", Pattern = "##.00" };
            }
        }
    }
}
