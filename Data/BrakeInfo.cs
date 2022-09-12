using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader {
    public class BrakeInfo {

        public Wheels brakePads = new Wheels();

        public void update(Physics physics) {
            brakePads.fl = physics.padLife[0];
            brakePads.fr = physics.padLife[1];
            brakePads.rl = physics.padLife[2];
            brakePads.rr = physics.padLife[3];
        }

        public void endLap(Physics physics) {
            brakePads.fl = physics.padLife[0];
            brakePads.fr = physics.padLife[1];
            brakePads.rl = physics.padLife[2];
            brakePads.rr = physics.padLife[3];
        }
    }
}
