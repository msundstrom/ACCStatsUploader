using System;
using System.Runtime.InteropServices;

namespace ACCStatsUploader {
    public class PhysicsEventArgs : EventArgs {
        public PhysicsEventArgs(Physics physics) {
            this.physics = physics;
        }

        public Physics physics { get; private set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Coordinates {
        public float X;
        public float Y;
        public float Z;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    [Serializable]
    public struct Physics {
        public int packetId;
        public float gas;
        public float brake;
        public float fuel;
        public int gear;
        public int rpms;
        public float steerAngle;
        public float speedKmh;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] velocity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] accG;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelSlip;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelLoad;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelsPressure;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelAngularSpeed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreWear;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreDirtyLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreCoreTemperature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] camberRad;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] suspensionTravel;

        public float drs;
        public float tc;
        public float heading;
        public float pitch;
        public float roll;
        public float cgHeight;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public float[] carDamage;

        public int numberOfTyresOut;
        public int pitLimiterOn;
        public float abs;

        public float kersCharge;
        public float kersInput;
        public int autoShifterOn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] rideHeight;

        // since 1.5
        public float turboBoost;
        public float ballast;
        public float airDensity;

        // since 1.6
        public float airTemp;
        public float roadTemp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] localAngularVelocity;
        public float finalFF;

        // since 1.7
        public float performanceMeter;
        public int engineBrake;
        public int ersRecoveryLevel;
        public int ersPowerLevel;
        public int ersHeatCharging;
        public int ersisCharging;
        public float kersCurrentKJ;
        public int drsAvailable;
        public int drsEnabled;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] brakeTemp;

        // since 1.10
        public float Clutch;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreTempI;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreTempM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreTempO;

        // since 1.10.2
        public int isAIControlled;

        // since 1.11
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Coordinates[] tyreContactPoint;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Coordinates[] tyreContactNormal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Coordinates[] tyreContactHeading;
        public float brakeBias;

        // since 1.12
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] localVelocity;

        // Not available
        public int P2PActivation;

        // Not available
        public int P2PStatus;

        // Not available
        public float currentMaxRpm;

        // Not available
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mz;

        // Not available
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] fx;

        // Not available
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] fy;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] slipRatio;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] slipAngle;

        // Not available
        public float tcInAction;

        // Not available
        public float absInAction;

        // Not available
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] suspensionDamage;

        // Not available
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreTemp;

        public float waterTemp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] brakePressure;

        public int fontBrakeCompound;
        public int rearBrakeCompound;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] padLife;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] discLife;

        public int ignitionOn;

        public int starterEngineOn;

        public int isEngineRunning;

        public float kerbVibration;
        public float slipVibrations;
        public float gVibrations;
        public float absVibrations;
    }
}
