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
            carDamage.front = physics.carDamage[0] / 3.5f;
            carDamage.rear = physics.carDamage[1] / 3.5f;
            carDamage.left = physics.carDamage[2] / 3.5f;
            carDamage.right = physics.carDamage[3] / 3.5f;

            suspensionDamage.front = physics.suspensionDamage[0] / 3.5f;
            suspensionDamage.rear = physics.suspensionDamage[1] / 3.5f;
            suspensionDamage.left = physics.suspensionDamage[2] / 3.5f;
            suspensionDamage.right = physics.suspensionDamage[3] / 3.5f;
        }
    }
}
