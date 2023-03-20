using ACCStatsUploader.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ACCStatsUploader.Data {
    public class ElectronicsInfo {
        private List<int> tcList = new List<int>();
        private List<int> tc2List = new List<int>();
        private List<int> absList = new List<int>();
        private List<int> fuelMapList = new List<int>();
        private List<float> brakeBiasList = new List<float>();
        private string carModel;

        public ElectronicsInfo(string carModel) { 
            this.carModel = carModel;
        }

        public string listTC() {
            return getGroupedListOutput(tcList);
        }

        public string listTC2() {
            return getGroupedListOutput(tc2List);
        }

        public string listABS() {
            return getGroupedListOutput(absList);
        }

        public string listFuelMap() {
            return getGroupedListOutput(fuelMapList);
        }

        public string averageBrakeBalance() { 

            return brakeBiasList.ToArray().Average().ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
        }
        public void update(Graphics graphics) {
            tcList.Add(graphics.tc);
            tc2List.Add(graphics.tcCut);
            absList.Add(graphics.abs);
            fuelMapList.Add(graphics.engineMap);
        }

        public void update(Physics physics) {
            brakeBiasList.Add(BrakeBalanceConverter.getBrakeBalance(this.carModel, physics.brakeBias));
        }

        public void endLap(Graphics graphics, Physics physics) {
            tcList.Add(graphics.tc);
            tc2List.Add(graphics.tcCut);
            absList.Add(graphics.abs);
            fuelMapList.Add(graphics.engineMap);
            brakeBiasList.Add(BrakeBalanceConverter.getBrakeBalance(this.carModel, physics.brakeBias));
        }

        // takes a list of electronic telemetry recordings collected over a lap, and returns a dictionary that groups together identical values and counts their occurrences.
        private SortedDictionary<string,float> createGroupedDict<T>(List<T> rawElectronics) {
            int totalCount = rawElectronics.Count;
            var unique = new SortedDictionary<string,float> ();
            foreach (var entry in rawElectronics) {
                string entryS = entry.ToString();
                if (!unique.ContainsKey(entryS)) {
                    int occurrence = rawElectronics.FindAll(x => x.Equals(entry)).Count();
                    float percentage = (float) occurrence / totalCount;
                    unique.Add(entryS, percentage);
                }
            }
            return unique;
        }

        string getGroupedListOutput<T>(List<T> input) {
            string output = "";
            var dict = createGroupedDict<T>(input);
            foreach(var item in dict) {
                string floatWithDot = item.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture); // converts 0,00000... to 0.00
                output += $"{item.Key}:{floatWithDot}, ";
            }
            output = output.Remove(output.Length - 2, 1); // remove the last comma
            return output;
        }
    }
}
