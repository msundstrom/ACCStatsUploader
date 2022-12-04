using System;
using System.Runtime.InteropServices;

namespace ACCStatsUploader {
    public enum ACC_FLAG_TYPE {
        ACC_NO_FLAG = 0,
        ACC_BLUE_FLAG = 1,
        ACC_YELLOW_FLAG = 2,
        ACC_BLACK_FLAG = 3,
        ACC_WHITE_FLAG = 4,
        ACC_CHECKERED_FLAG = 5,
        ACC_PENALTY_FLAG = 6
    }

    public enum ACC_STATUS {
        ACC_OFF = 0,
        ACC_REPLAY = 1,
        ACC_LIVE = 2,
        ACC_PAUSE = 3
    };

    public enum ACC_SESSION_TYPE {
        ACC_UNKNOWN = -1,
        ACC_PRACTICE = 0,
        ACC_QUALIFY = 1,
        ACC_RACE = 2,
        ACC_HOTLAP = 3,
        ACC_TIME_ATTACK = 4,
        ACC_DRIFT = 5,
        ACC_DRAG = 6
    };

    public enum ACC_TRACK_GRIP_STATUS {
        ACC_GREEN = 0,
        ACC_FAST = 1,
        ACC_OPTIMUM = 2,
        ACC_GREASY = 3,
        ACC_DAMP = 4,
        ACC_WET = 5,
        ACC_FLOODED = 6
    };

    public enum ACC_RAIN_INTENSITY {
        ACC_NO_RAIN = 0,
        ACC_DRIZZLE = 1,
        ACC_LIGHT_RAIN = 2,
        ACC_MEDIUM_RAIN = 3,
        ACC_HEAVY_RAIN = 4,
        ACC_THUNDERSTORM = 5
    };

    public enum ACC_PENALTY_TYPE {
        None = 0,
        DriveThrough_Cutting = 1,
        StopAndGo_10_Cutting = 2,
        StopAndGo_20_Cutting = 3,
        StopAndGo_30_Cutting = 4,
        Disqualified_Cutting = 5,
        RemoveBestLaptime_Cutting = 6,

        DriveThrough_PitSpeeding = 7,
        StopAndGo_10_PitSpeeding = 8,
        StopAndGo_20_PitSpeeding = 9,
        StopAndGo_30_PitSpeeding = 10,
        Disqualified_PitSpeeding = 11,
        RemoveBestLaptime_PitSpeeding = 12,

        Disqualified_IgnoredMandatoryPit = 13,

        PostRaceTime = 14,
        Disqualified_Trolling = 15,
        Disqualified_PitEntry = 16,
        Disqualified_PitExit = 17,
        Disqualified_WrongWay = 18,

        DriveThrough_IgnoredDriverStint = 19,
        Disqualified_IgnoredDriverStint = 20,

        Disqualified_ExceededDriverStintLimit = 21,
    };

    public class GraphicsEventArgs : EventArgs {
        public GraphicsEventArgs(Graphics graphics) {
            this.graphics = graphics;
        }

        public Graphics graphics { get; private set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    [Serializable]
    public struct Graphics {
        public int packetId;
        public ACC_STATUS status;
        public ACC_SESSION_TYPE session;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String currentTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String lastTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String bestTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String split;

        public int completedLaps;
        public int position;
        public int iCurrentTime;
        public int iLastTime;
        public int iBestTime;
        public float sessionTimeLeft;
        public float distanceTraveled;
        public int isInPit;
        public int currentSectorIndex;
        public int lastSectorTime;
        public int numberOfLaps;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String TyreCompound;
        public float replayTimeMultiplier;
        public float normalizedCarPosition;

        public int activeCars;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 180)]
        public float[] carCoordinates;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public int[] carId;
        public int playerCarID;
        public float penaltyTime;
        public ACC_FLAG_TYPE flag;
        public ACC_PENALTY_TYPE penalty;
        public int idealLineOn;
        public int isInPitLane;
        public float surfaceGrip;
        public int mandatoryPitDone;
        public float windSpeed;
        public float windDirection;
        public int isSetupMenuVisible;
        public int mainDisplayIndex;
        public int secondaryDisplayIndex;
        public int tc;
        public int tcCut;
        public int engineMap;
        public int abs;
        public int fuelXLap;
        public int rainLights;
        public int flashingLights;
        public int lightsStage;
        public float exhaustTemperature;
        public int wiperLV;
        public int driverStintTotalTimeLeft;
        public int driverStintTimeLeft;
        public int rainTyres;
        public int sessionIndex;
        public float usedFuel;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String deltaLapTime;
        public int iDeltaLapTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String estimatedLapTime;
        public int iEstimatedLapTime;
        public int isDeltaPositive;
        public int iSplit;
        public int isValidLap;
        public float fuelEstimatedLaps;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String trackStatus;
        public int missingMandatoryPits;
        public float Clock;
        public int directionLightsLeft;
        public int directionLightsRight;
        public int globalYellow;
        public int globalYellow1;
        public int globalYellow2;
        public int globalYellow3;
        public int globalWhite;
        public int globalGreen;
        public int globalChequered;
        public int globalRed;
        public int mfdTyreSet;
        public float mfdFuelToAdd;
        public float mfdTyrePressureLF;
        public float mfdTyrePressureRF;
        public float mfdTyrePressureLR;
        public float mfdTyrePressureRR;
        public ACC_TRACK_GRIP_STATUS trackGripStatus;
        public ACC_RAIN_INTENSITY rainIntensity;
        public ACC_RAIN_INTENSITY rainIntensityIn10min;
        public ACC_RAIN_INTENSITY rainIntensityIn30min;
        public int currentTyreSet;
        public int strategyTyreSet;
    }
}
