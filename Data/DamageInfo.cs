namespace ACCStatsUploader {
    public class DamageInfo {
        public struct Location {
            public float front;
            public float right;
            public float rear;
            public float left;
        }

        public Location carDamage = new Location();
        public Location suspensionDamage = new Location();
        

        public void endLap(Physics physics) {
            carDamage.front = physics.carDamage[0];
            carDamage.rear = physics.carDamage[1];
            carDamage.left = physics.carDamage[2];
            carDamage.right = physics.carDamage[3];

            suspensionDamage.front = physics.suspensionDamage[0];
            suspensionDamage.rear = physics.suspensionDamage[1];
            suspensionDamage.left = physics.suspensionDamage[2];
            suspensionDamage.right = physics.suspensionDamage[3];
        }
    }
}
