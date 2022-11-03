using UnityEngine;

namespace Physics
{
    public class Gravity
    {
        public Gravity(float forceValue)
        {
            _forceValue = forceValue;
        }
        
        private readonly float _forceValue;
        private float _accumulatedVelocity;

        public float ForceValue => _forceValue;
        public float AccumulatedVelocity => _accumulatedVelocity;

        public void ResetForce()
        {
            _accumulatedVelocity = 0;
        }

        public float AddForce()
        {
            _accumulatedVelocity += _forceValue * Time.deltaTime;
            if (Mathf.Abs(_accumulatedVelocity) < 0.1) _accumulatedVelocity = 0.1f * Mathf.Sign(_forceValue);
            return _accumulatedVelocity;
        }
    }
}