using Google.Apis.Sheets.v4.Data;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACCStatsUploader {
    public class TyreSetController {
        private FileSystemWatcher fileSystemWatcher;
        private String carDumpJson = "swap_dump_carjson.json";
        private TyreSetsSheet tyreSetSheet;
        private int lastUsedTyreSet;

        private string tyreDumpFolderPath() {
            var documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            documentsFolderPath += "/Assetto Corsa Competizione/Debug/";
            return documentsFolderPath;
        }

        private string tyreDumpFilePath() {
            return tyreDumpFolderPath() + carDumpJson;
        }

        public void start(TyreSetsSheet tyreSetSheet) {
            this.tyreSetSheet = tyreSetSheet;
            var fileExists = FileSystem.FileExists(tyreDumpFilePath());

            fileSystemWatcher = new FileSystemWatcher(tyreDumpFolderPath());
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Filter = carDumpJson;

            fileSystemWatcher.Changed += onChanged;
            
            // debug stuff
            /*
            var carDump = deserializeCarDump(readCarDumpFile());
            if (carDump != null) {
                var dmp = (CarDumpJson.RootObject)carDump;
                tyreSetSheet.updateTyreSetData(dmp.tyreSets);
            } else {
                Debug.WriteLine("Could not deseralize CarDump");
            }
            */
        }

        public void update(Graphics graphics, Physics physics, StaticInfo staticInfo) {
            if (graphics.rainTyres == 1) {
                lastUsedTyreSet = 0;
            } else {
                lastUsedTyreSet = graphics.currentTyreSet;
            }
        }

        private void onChanged(object sender, FileSystemEventArgs e) {
            var carDump = deserializeCarDump(readCarDumpFile());
            if (carDump != null) {
                var dmp = (CarDumpJson.RootObject)carDump;
                tyreSetSheet.addTyreSetData(dmp.tyreSets[lastUsedTyreSet]);
            } else {
                Debug.WriteLine("Could not deseralize CarDump");
            }
        }

        private string readCarDumpFile() {
            return File.ReadAllText(tyreDumpFilePath());
        }

        private CarDumpJson.RootObject? deserializeCarDump(string carDump) {
            return JsonConvert.DeserializeObject<CarDumpJson.RootObject>(readCarDumpFile());
        }
    }

    public class CarDumpJson {
        public struct RootObject {
            public int carId { get; set; }
            public int driverIndex { get; set; }
            public int oldPhysicsTimeMs { get; set; }

            public Strategy strategy { get; set; }

            public PitSpeedingDetector pitSpeedingDetector { get; set; }
            public PitstopMFD pitStopMfd { get; set; }

            public Fuel fuel { get; set; }

            public List<TyreSet> tyreSets { get; set; }
            public List<TyreSet> startingTyreSets { get; set; }
            public int currentTyreSet { get; set; }
            public int curentTyreCompound { get; set; }
            public int currentFrontBrakeCompound { get; set; }
            public int currentRearBrakeCompound { get; set; }
            public double lastSplitNpos { get; set; }
            public int lapCount { get; set; }
            public bool hotlapFirstLapArmed { get; set; }
            public int filteredTimeAhead { get; set; }
            public double averageRelativeSpeed { get; set; }
            public CarSetup carSetup { get; set; }
        }
        

        public struct PitStrategy {
            public int fuelToAdd { get; set; }
            public Tyres tyres { get; set; }
            public int tyreSet { get; set; }
            public int frontBrakeCompound { get; set; }
            public int rearBrakeCompound { get; set; }
        }

        public struct MFDStrategy {
            public int fuel { get; set; }
            public int nrOfPitStops { get; set; }
            public int tyreSet { get; set; }
            public int frontBrakeCompound { get; set; }
            public int rearBrakeCompound { get; set; }
            public List<PitStrategy> pitStrategy { get; set; }
        }

        public struct Electronics {
            public int tc1 { get; set; }
            public int tc2 { get; set; }
            public int abs { get; set; }
            public int ecuMap { get; set; }
            public int fuelMix { get; set; }
            public int telemetryLaps { get; set; }
        }

        public struct Alignment {
            public List<Double> camber { get; set; }
            public List<Double> toe { get; set; }
            public List<Double> staticCamber { get; set; }
            public List<Double> toeOutLinear { get; set; }
            public int casterLF { get; set; }
            public int casterRF { get; set; }
            public int steerRatio { get; set; }
        }

        public struct Tyres {
            public int tyreCompound { get; set; }
            public List<Double> tyrePressure { get; set; }
        }

        public struct BasicSetup {
            public Tyres tyres { get; set; }
            public Alignment alignment { get; set; }
            public Electronics electronics { get; set; }
            public MFDStrategy strategy { get; set; }
        }

        public struct AdvancedSetup {
            public MechanicalBalance mechanicalBalance { get; set; }
            public Dampers dampers { get; set; }
            public AeroBalance aeroBalance { get; set; }
            public DriveTrain driveTrain { get; set; }
        }

        public struct MechanicalBalance {
            public int arbFront { get; set; }
            public int arbRear { get; set; }
            public List<int> wheelRate { get; set; }
            public List<int> bumpStopRateUp { get; set; }
            public List<int> bumpStopRateDown { get; set; }
            public List<int> bumpStopWindow { get; set; }
            public int brakeTorque { get; set; }
            public int brakeBias { get; set; }
        }

        public struct Dampers {
            public List<int> bumpSlow { get; set; }
            public List<int> bumpFast { get; set; }
            public List<int> reboundSlow { get; set; }
            public List<int> reboundFast { get; set; }
        }

        public struct AeroBalance {
            public List<int> rideheight { get; set; }
            public List<Double> rodLength { get; set; }
            public int splitter { get; set; }
            public int rearWing { get; set; }
            public List<int> brakeDucts { get; set; }
        }

        public struct DriveTrain {
            public int preload { get; set; }
        }

        public struct CarSetup {
            public string carName { get; set; }
            public int trackBopType { get; set; }
            public BasicSetup basicSetup { get; set; }
            public AdvancedSetup advancedSetup { get; set; }
        }

        public struct Strategy {
            public string pitstopReason { get; set; }
            public bool isStartTimeSet { get; set; }
            public int startTimeMs { get; set; }
            public int pitEntryPitLaneSessionTimeMs { get; set; }
            public int pitExitPitlaneSessionTimeMs { get; set; }
            public int fastPosition { get; set; }
            public CarSetup carSetup { get; set; }
            public int nextPitstopAtSesstionTimeMs { get; set; }
            public int mandatoryPitstopNumber { get; set; }
            public double fuelPerLap { get; set; }
            public double fuelPerMinute { get; set; }
            public int lapTime { get; set; }
            public int rainPitRandomizer { get; set; }
            public int pitStopCounter { get; set; }
            public List<double> driverTotalTimes { get; set; }
        }

        public struct PitSpeedingDetector {
            public double timeOverLimitS { get; set; }
            public double maxSpeedKmh { get; set; }
        }

        public struct PitstopMFD {
            public int activePitStrategy { get; set; }
            public int tyreSet { get; set; }
            public bool isDisabled { get; set; }
            public int fuelToAdd { get; set; }
            public FuelLimits fuelLimits { get; set; }
            public bool isRefuellingAllowed { get; set; }
            public int newTyreCompound { get; set; }
            public List<String> tyreCompoundNames { get; set; }
            public List<bool> tyreToChange { get; set; }
            public List<double> tyrePressures { get; set; }
            public TyrePressureLimits tyrePressureLimits { get; set; }
            public bool driverSwapAllowed { get; set; }
            public int newDriverIndexForSwap { get; set; }
            public string newDriverNameToDisplay { get; set; }
            public List<string> driverNames { get; set; }
            public List<int> driversStintTotalTimeLeft { get; set; }
            public List<bool> hasDriversDoneAStint { get; set; }
            public int currentDriverIndex { get; set; }
            public bool repairBody { get; set; }
            public bool repairSuspention { get; set; }
            public bool repairEngine { get; set; }
            public double timeRequired { get; set; }
            public bool mustServePenalty { get; set; }
            public bool changeBrakeDiscs { get; set; }
            public int frontBrakeCompound { get; set; }
            public int rearBrakeCompound { get; set; }
            public bool isValidForMandatory { get; set; }
            public bool isDriverSwapRequired { get; set; }
            public bool isTyreChangeRequired { get; set; }
            public bool isRefuellingRquired { get; set; }
            public bool isPitEntryValid { get; set; }
            public bool hasPitWindowRule { get; set; }
            public bool hasDriverStintRule { get; set; }
            public bool isDriverStintCompleted { get; set; }
            public int missingMandatoryPitStops { get; set; }
            public int baseMandatoryPitStops { get; set; }
        }

        public struct FuelLimits {
            public int x { get; set; }
            public int y { get; set; }
        }

        public struct TyrePressureLimits {
            public double x { get; set; }
            public double y { get; set; }
        }

        public struct Fuel {
            public double startingFuelOnLastLap { get; set; }
            public double totalFuelUsedInSession { get; set; }
            public double fuelPerSingleLap { get; set; }
            public double fuelPerKm { get; set; }
            public int fuelLapCount { get; set; }
        }

        public struct TyreSet {
            public int tyreSet { get; set; }
            public List<TyreWearStatus> wearStatus { get; set; }
            public string state { get; set; }
        }

        public struct TyreWearStatus {
            public List<double> treadMM { get; set; }
            public double grain { get; set; }
            public double blister { get; set; }
            public double marblesLevel { get; set; }
            public double flatSpot { get; set; }
            public bool isCriticalState { get; set; }
        }
    }
}
