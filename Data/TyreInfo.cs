using System.Collections.Generic;
using System.Linq;

namespace ACCStatsUploader {
    public class TyreInfo {
        public List<Wheels> tyrePressures = new List<Wheels>();
        public List<Wheels> tyreTemperatures = new List<Wheels>();

        public Wheels maxPressures() {
            double flMax = tyrePressures.Select(wheel => wheel.fl).ToArray().Max();
            double frMax = tyrePressures.Select(wheel => wheel.fr).ToArray().Max();
            double rlMax = tyrePressures.Select(wheel => wheel.rl).ToArray().Max();
            double rrMax = tyrePressures.Select(wheel => wheel.rr).ToArray().Max();

            return new Wheels(flMax, frMax, rlMax, rrMax);
        }

        public Wheels averagePressures() {
            double flAvg = tyrePressures.Select(wheel => wheel.fl).ToArray().Average();
            double frAvg = tyrePressures.Select(wheel => wheel.fr).ToArray().Average();
            double rlAvg = tyrePressures.Select(wheel => wheel.rl).ToArray().Average();
            double rrAvg = tyrePressures.Select(wheel => wheel.rr).ToArray().Average();

            return new Wheels(flAvg, frAvg, rlAvg, rrAvg);
        }

        public Wheels maxTemps() {
            double flMax = tyreTemperatures.Select(wheel => wheel.fl).ToArray().Max();
            double frMax = tyreTemperatures.Select(wheel => wheel.fr).ToArray().Max();
            double rlMax = tyreTemperatures.Select(wheel => wheel.rl).ToArray().Max();
            double rrMax = tyreTemperatures.Select(wheel => wheel.rr).ToArray().Max();

            return new Wheels(flMax, frMax, rlMax, rrMax);
        }

        public Wheels averageTemps() {
            double flAvg = tyreTemperatures.Select(wheel => wheel.fl).ToArray().Average();
            double frAvg = tyreTemperatures.Select(wheel => wheel.fr).ToArray().Average();
            double rlAvg = tyreTemperatures.Select(wheel => wheel.rl).ToArray().Average();
            double rrAvg = tyreTemperatures.Select(wheel => wheel.rr).ToArray().Average();

            return new Wheels(flAvg, frAvg, rlAvg, rrAvg);
        }

        public void update(Physics physicsUpdate) {
            tyrePressures.Add(
                new Wheels(
                    physicsUpdate.wheelsPressure[0],
                    physicsUpdate.wheelsPressure[1],
                    physicsUpdate.wheelsPressure[2],
                    physicsUpdate.wheelsPressure[3]
                )
            );

            tyreTemperatures.Add(
                new Wheels(
                    physicsUpdate.brakeTemp[0],
                    physicsUpdate.brakeTemp[1],
                    physicsUpdate.brakeTemp[2],
                    physicsUpdate.brakeTemp[3]
                )
            );
        }
    }
}