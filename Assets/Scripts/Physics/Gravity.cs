using UnityEngine;

namespace Physics {
    public class Gravity {
        public Gravity(float forceValue) {
            ForceValue = forceValue;
        }

        public float ForceValue { get; }

        public float AccumulatedVelocity { get; private set; }

        public void ResetForce() {
            AccumulatedVelocity = 0;
        }

        public float AddForce(float deltaTime) {
            AccumulatedVelocity += ForceValue * deltaTime;
            if (Mathf.Abs(AccumulatedVelocity) < 0.1) AccumulatedVelocity = 0.1f * Mathf.Sign(ForceValue);
            return AccumulatedVelocity;
        }
    }
}
