using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACCStatsUploader.Converters.CarModel;

namespace ACCStatsUploader.Converters {
    public enum CarModelID {
        AMR_V12_VANTAGE_GT3,
        AUDI_R8_LMS,
        BENTLEY_CONTINENTAL_GT3_2016,
        BENTLEY_CONTINENTAL_GT3_2018,
        BMW_M6_GT3,
        JAGUAR_G3,
        FERRARI_488_GT3,
        HONDA_NSX_GT3,
        LAMBORGHINI_GALLARDO_REX,
        LAMBORGHINI_HURACAN_GT3,
        LAMBORGHINI_HURACAN_ST,
        LEXUS_RC_F_GT3,
        MCLAREN_650S_GT3,
        MERCEDES_AMG_GT3,
        NISSAN_GT_R_GT3_2017,
        NISSAN_GT_R_GT3_2018,
        PORSCHE_991_GT3_R,
        PORSCHE_991II_GT3_CUP,
        AMR_V8_VANTAGE_GT3,
        AUDI_R8_LMS_EVO,
        HONDA_NSX_GT3_EVO,
        LAMBORGHINI_HURACAN_GT3_EVO,
        MCLAREN_720S_GT3,
        PORSCHE_991II_GT3_R,
        ALPINE_A110_GT4,
        AMR_V8_VANTAGE_GT4,
        AUDI_R8_GT4,
        BMW_M4_GT4,
        CHEVROLET_CAMARO_GT4R,
        GINETTA_G55_GT4,
        KTM_XBOW_GT4,
        MASERATI_MC_GT4,
        MCLAREN_570S_GT4,
        MERCEDES_AMG_GT4,
        PORSCHE_718_CAYMAN_GT4_MR,
        FERRARI_488_GT3_EVO,
        MERCEDES_AMG_GT3_EVO,
        BMW_M4_GT3,
        AUDI_R8_LMS_EVO_II,
        BMW_M2_CS_RACING,
        FERRARI_488_CHALLENGE_EVO,
        LAMBORGHINI_HURACAN_ST_EVO2,
        PORSCHE_992_GT3_CUP,
        NO_MODEL
    }

    public class CarModel {
        public static CarModelID StringToObject(string carModelString) {
            switch(carModelString) {
                case "ALPINE_A110_GT4": return CarModelID.ALPINE_A110_GT4;
                case "AMR_V12_VANTAGE_GT3": return CarModelID.AMR_V12_VANTAGE_GT3;
                case "AMR_V8_VANTAGE_GT3": return CarModelID.AMR_V8_VANTAGE_GT3;
                case "AMR_V8_VANTAGE_GT4": return CarModelID.AMR_V8_VANTAGE_GT4;
                case "AUDI_R8_GT4": return CarModelID.AUDI_R8_GT4;
                case "AUDI_R8_LMS": return CarModelID.AUDI_R8_LMS;
                case "AUDI_R8_LMS_EVO": return CarModelID.AUDI_R8_LMS_EVO;
                case "AUDI_R8_LMS_EVO_II": return CarModelID.AUDI_R8_LMS_EVO_II;
                case "BENTLEY_CONTINENTAL_GT3_2016": return CarModelID.BENTLEY_CONTINENTAL_GT3_2016;
                case "BENTLEY_CONTINENTAL_GT3_2018": return CarModelID.BENTLEY_CONTINENTAL_GT3_2018;
                case "BMW_M2_CS_RACING": return CarModelID.BMW_M2_CS_RACING;
                case "BMW_M4_GT3": return CarModelID.BMW_M4_GT3;
                case "BMW_M4_GT4": return CarModelID.BMW_M4_GT4;
                case "BMW_M6_GT3": return CarModelID.BMW_M6_GT3;
                case "CHEVROLET_CAMARO_GT4R": return CarModelID.CHEVROLET_CAMARO_GT4R;
                case "FERRARI_488_CHALLENGE_EVO": return CarModelID.FERRARI_488_CHALLENGE_EVO;
                case "FERRARI_488_GT3": return CarModelID.FERRARI_488_GT3;
                case "FERRARI_488_GT3_EVO": return CarModelID.FERRARI_488_GT3_EVO;
                case "GINETTA_G55_GT4": return CarModelID.GINETTA_G55_GT4;
                case "HONDA_NSX_GT3": return CarModelID.HONDA_NSX_GT3;
                case "HONDA_NSX_GT3_EVO": return CarModelID.HONDA_NSX_GT3_EVO;
                case "JAGUAR_G3": return CarModelID.JAGUAR_G3;
                case "KTM_XBOW_GT4": return CarModelID.KTM_XBOW_GT4;
                case "LAMBORGHINI_GALLARDO_REX": return CarModelID.LAMBORGHINI_GALLARDO_REX;
                case "LAMBORGHINI_HURACAN_GT3": return CarModelID.LAMBORGHINI_HURACAN_GT3;
                case "LAMBORGHINI_HURACAN_GT3_EVO": return CarModelID.LAMBORGHINI_HURACAN_GT3_EVO;
                case "LAMBORGHINI_HURACAN_ST": return CarModelID.LAMBORGHINI_HURACAN_ST;
                case "LAMBORGHINI_HURACAN_ST_EVO2": return CarModelID.LAMBORGHINI_HURACAN_ST_EVO2;
                case "LEXUS_RC_F_GT3": return CarModelID.LEXUS_RC_F_GT3;
                case "MASERATI_MC_GT4": return CarModelID.MASERATI_MC_GT4;
                case "MCLAREN_570S_GT4": return CarModelID.MCLAREN_570S_GT4;
                case "MCLAREN_650S_GT3": return CarModelID.MCLAREN_650S_GT3;
                case "MCLAREN_720S_GT3": return CarModelID.MCLAREN_720S_GT3;
                case "MERCEDES_AMG_GT3": return CarModelID.MERCEDES_AMG_GT3;
                case "MERCEDES_AMG_GT3_EVO": return CarModelID.MERCEDES_AMG_GT3_EVO;
                case "MERCEDES_AMG_GT4": return CarModelID.MERCEDES_AMG_GT4;
                case "NISSAN_GT_R_GT3_2017": return CarModelID.NISSAN_GT_R_GT3_2017;
                case "NISSAN_GT_R_GT3_2018": return CarModelID.NISSAN_GT_R_GT3_2018;
                case "PORSCHE_718_CAYMAN_GT4_MR": return CarModelID.PORSCHE_718_CAYMAN_GT4_MR;
                case "PORSCHE_991_GT3_R": return CarModelID.PORSCHE_991_GT3_R;
                case "PORSCHE_991II_GT3_CUP": return CarModelID.PORSCHE_991II_GT3_CUP;
                case "PORSCHE_991II_GT3_R": return CarModelID.PORSCHE_991II_GT3_R;
                case "PORSCHE_992_GT3_CUP": return CarModelID.PORSCHE_992_GT3_CUP;
                default: return CarModelID.NO_MODEL;
            }
        }
    }
}
