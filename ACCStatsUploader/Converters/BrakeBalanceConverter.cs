using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.Converters {
    public class BrakeBalanceConverter {
        private static int getBrakeBalanceOffset(CarModelID carModelID) {
           switch(carModelID) {
                case CarModelID.ALPINE_A110_GT4: return -15;
                case CarModelID.AMR_V12_VANTAGE_GT3: return -7;
                case CarModelID.AMR_V8_VANTAGE_GT3: return -7;
                case CarModelID.AMR_V8_VANTAGE_GT4: return -20;
                case CarModelID.AUDI_R8_GT4: return -15;
                case CarModelID.AUDI_R8_LMS: return -14;
                case CarModelID.AUDI_R8_LMS_EVO: return -14;
                case CarModelID.AUDI_R8_LMS_EVO_II: return -14;
                case CarModelID.BENTLEY_CONTINENTAL_GT3_2016: return -7;
                case CarModelID.BENTLEY_CONTINENTAL_GT3_2018: return -7;
                case CarModelID.BMW_M2_CS_RACING: return -17;
                case CarModelID.BMW_M4_GT3: return -14;
                case CarModelID.BMW_M4_GT4: return -22;
                case CarModelID.BMW_M6_GT3: return -15;
                case CarModelID.CHEVROLET_CAMARO_GT4R: return -18;
                case CarModelID.FERRARI_488_CHALLENGE_EVO: return -13;
                case CarModelID.FERRARI_488_GT3: return -17;
                case CarModelID.FERRARI_488_GT3_EVO: return -17;
                case CarModelID.GINETTA_G55_GT4: return -18;
                case CarModelID.HONDA_NSX_GT3: return -14;
                case CarModelID.HONDA_NSX_GT3_EVO: return -14;
                case CarModelID.JAGUAR_G3: return -7;
                case CarModelID.KTM_XBOW_GT4: return -20;
                case CarModelID.LAMBORGHINI_GALLARDO_REX: return -14;
                case CarModelID.LAMBORGHINI_HURACAN_GT3: return -14;
                case CarModelID.LAMBORGHINI_HURACAN_GT3_EVO: return -14;
                case CarModelID.LAMBORGHINI_HURACAN_ST: return -14;
                case CarModelID.LAMBORGHINI_HURACAN_ST_EVO2: return -14;
                case CarModelID.LEXUS_RC_F_GT3: return -14;
                case CarModelID.MASERATI_MC_GT4: return -15;
                case CarModelID.MCLAREN_570S_GT4: return -9;
                case CarModelID.MCLAREN_650S_GT3: return -17;
                case CarModelID.MCLAREN_720S_GT3: return -17;
                case CarModelID.MERCEDES_AMG_GT3: return -14;
                case CarModelID.MERCEDES_AMG_GT3_EVO: return -14;
                case CarModelID.MERCEDES_AMG_GT4: return -20;
                case CarModelID.NISSAN_GT_R_GT3_2017: return -15;
                case CarModelID.NISSAN_GT_R_GT3_2018: return -15;
                case CarModelID.PORSCHE_718_CAYMAN_GT4_MR: return -20;
                case CarModelID.PORSCHE_991_GT3_R: return -21;
                case CarModelID.PORSCHE_991II_GT3_CUP: return -5;
                case CarModelID.PORSCHE_991II_GT3_R: return -21;
                case CarModelID.PORSCHE_992_GT3_CUP: return -5;
                default: return 0;
            }
        }

        public static float getBrakeBalance(string carModelString, float rawBrakeBias) {
            CarModelID carModel = CarModel.StringToObject(carModelString);
            int brakeBalanceOffsetValue = getBrakeBalanceOffset(carModel);
            return rawBrakeBias * 100 + brakeBalanceOffsetValue;
        }
    }
}
