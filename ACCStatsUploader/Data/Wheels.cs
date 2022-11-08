namespace ACCStatsUploader {
    public struct Wheels {
        public double fr;
        public double fl;
        public double rl;
        public double rr;

        public Wheels(double frontRight, double frontLeft, double rearLeft, double rearRight) {
            fr = frontRight;
            fl = frontLeft;
            rl = rearLeft;
            rr = rearRight;
        }
    }
}
