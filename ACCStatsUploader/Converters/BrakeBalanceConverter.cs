using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.Converters {
    public class BrakeBalanceConverter {
        private static Dictionary<CarModel.KUNOS_ID, int> brakeBalanceOffset = new Dictionary<CarModel.KUNOS_ID, int> {
            {CarModel.KUNOS_ID.AMR_V12_VANTAGE_GT3,-7},
            {CarModel.KUNOS_ID.AUDI_R8_LMS,-14},
            {CarModel.KUNOS_ID.BENTLEY_CONTINENTAL_GT3_2016,-7},
            {CarModel.KUNOS_ID.BENTLEY_CONTINENTAL_GT3_2018,-7},
            {CarModel.KUNOS_ID.BMW_M6_GT3,-15},
            {CarModel.KUNOS_ID.JAGUAR_G3,-7},
            {CarModel.KUNOS_ID.FERRARI_488_GT3,-17},
            {CarModel.KUNOS_ID.HONDA_NSX_GT3,-14},
            {CarModel.KUNOS_ID.LAMBORGHINI_GALLARDO_REX,-14},
            {CarModel.KUNOS_ID.LAMBORGHINI_HURACAN_GT3,-14},
            {CarModel.KUNOS_ID.LAMBORGHINI_HURACAN_ST,-14},
            {CarModel.KUNOS_ID.LEXUS_RC_F_GT3,-14},
            {CarModel.KUNOS_ID.MCLAREN_650S_GT3,-17},
            {CarModel.KUNOS_ID.MERCEDES_AMG_GT3,-14},
            {CarModel.KUNOS_ID.NISSAN_GT_R_GT3_2017,-15},
            {CarModel.KUNOS_ID.NISSAN_GT_R_GT3_2018,-15},
            {CarModel.KUNOS_ID.PORSCHE_991_GT3_R,-21},
            {CarModel.KUNOS_ID.PORSCHE_991II_GT3_CUP,-5},
            {CarModel.KUNOS_ID.AMR_V8_VANTAGE_GT3,-7},
            {CarModel.KUNOS_ID.AUDI_R8_LMS_EVO,-14},
            {CarModel.KUNOS_ID.HONDA_NSX_GT3_EVO,-14},
            {CarModel.KUNOS_ID.LAMBORGHINI_HURACAN_GT3_EVO,-14},
            {CarModel.KUNOS_ID.MCLAREN_720S_GT3,-17},
            {CarModel.KUNOS_ID.PORSCHE_991II_GT3_R,-21},
            {CarModel.KUNOS_ID.ALPINE_A110_GT4,-15},
            {CarModel.KUNOS_ID.AMR_V8_VANTAGE_GT4,-20},
            {CarModel.KUNOS_ID.AUDI_R8_GT4,-15},
            {CarModel.KUNOS_ID.BMW_M4_GT4,-22},
            {CarModel.KUNOS_ID.CHEVROLET_CAMARO_GT4R,-18},
            {CarModel.KUNOS_ID.GINETTA_G55_GT4,-18},
            {CarModel.KUNOS_ID.KTM_XBOW_GT4,-20},
            {CarModel.KUNOS_ID.MASERATI_MC_GT4,-15},
            {CarModel.KUNOS_ID.MCLAREN_570S_GT4,-9},
            {CarModel.KUNOS_ID.MERCEDES_AMG_GT4,-20},
            {CarModel.KUNOS_ID.PORSCHE_718_CAYMAN_GT4_MR,-20},
            {CarModel.KUNOS_ID.FERRARI_488_GT3_EVO,-17},
            {CarModel.KUNOS_ID.MERCEDES_AMG_GT3_EVO,-14},
            {CarModel.KUNOS_ID.BMW_M4_GT3,-14},
            {CarModel.KUNOS_ID.AUDI_R8_LMS_EVO_II,-14},
            {CarModel.KUNOS_ID.BMW_M2_CS_RACING,-17},
            {CarModel.KUNOS_ID.FERRARI_488_CHALLENGE_EVO,-13},
            {CarModel.KUNOS_ID.LAMBORGHINI_HURACAN_ST_EVO2,-14},
            {CarModel.KUNOS_ID.PORSCHE_992_GT3_CUP,-5}
        };

        public static float getBrakeBalance(string carModel, float rawBrakeBias) {
            CarModel.KUNOS_ID kunosCarID = CarModel.carModelID.GetValueOrDefault(carModel);
            int brakeBalanceOffsetValue = brakeBalanceOffset.GetValueOrDefault(kunosCarID);
            return rawBrakeBias * 100 + brakeBalanceOffsetValue;
        }
    }
}
