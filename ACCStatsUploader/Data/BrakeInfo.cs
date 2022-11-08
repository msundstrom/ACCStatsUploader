using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    public class BrakeInfo {

        public Wheels brakePads = new Wheels();
        private List<Wheels> brakeTemperatures = new List<Wheels>();

        public void update(Physics physics) {
            brakePads.fl = physics.padLife[0];
            brakePads.fr = physics.padLife[1];
            brakePads.rl = physics.padLife[2];
            brakePads.rr = physics.padLife[3];

            brakePads.fl = physics.brakeTemp[0];
            brakePads.fr = physics.brakeTemp[1];
            brakePads.rl = physics.brakeTemp[2];
            brakePads.rr = physics.brakeTemp[3];
        }
        public Wheels maxTemps() {
            double flMax = brakeTemperatures.Select(wheel => wheel.fl).ToArray().Max();
            double frMax = brakeTemperatures.Select(wheel => wheel.fr).ToArray().Max();
            double rlMax = brakeTemperatures.Select(wheel => wheel.rl).ToArray().Max();
            double rrMax = brakeTemperatures.Select(wheel => wheel.rr).ToArray().Max();

            return new Wheels(flMax, frMax, rlMax, rrMax);
        }

        public Wheels averageTemps() {
            double flAvg = brakeTemperatures.Select(wheel => wheel.fl).ToArray().Average();
            double frAvg = brakeTemperatures.Select(wheel => wheel.fr).ToArray().Average();
            double rlAvg = brakeTemperatures.Select(wheel => wheel.rl).ToArray().Average();
            double rrAvg = brakeTemperatures.Select(wheel => wheel.rr).ToArray().Average();

            return new Wheels(flAvg, frAvg, rlAvg, rrAvg);
        }

        public void endLap(Physics physics) {
            brakePads.fl = physics.padLife[0];
            brakePads.fr = physics.padLife[1];
            brakePads.rl = physics.padLife[2];
            brakePads.rr = physics.padLife[3];

            brakeTemperatures.Add(
                new Wheels(
                    physics.brakeTemp[0],
                    physics.brakeTemp[1],
                    physics.brakeTemp[2],
                    physics.brakeTemp[3]
                )
            );
        }
    }
}
