using Combat;
using UnityEngine;

namespace Effects {
    public class ShakeEffect : Effect {
        private Vector3 _offset;
        private float _amplitude;
        private float _frequency;
        private float _buildUpTime;
        private float _easeOutTime;

        private float _currentAmp;
        private float _currentFreq;

        private float Duration => _buildUpTime + _easeOutTime;
        private float Timer => Duration - TimeToLive;
        private float EaseOutPoint => Duration - _easeOutTime;

        public static GameObject Spawn(
            float buildUpTime,
            float easeOutTime,
            float amplitude,
            float frequency,
            Vector3 offset
        ) {
            var obj = new GameObject("ShakeEffect");
            var effect = obj.AddComponent<ShakeEffect>();
            effect._offset = offset;
            effect._amplitude = amplitude;
            effect._frequency = frequency;
            effect._buildUpTime = buildUpTime;
            effect._easeOutTime = easeOutTime;
            effect.TimeToLive = effect.Duration;
            return obj;
        }

        public static GameObject Spawn(
            float buildUpTime,
            float easeOutTime,
            float amplitude,
            float frequency
        ) {
            return Spawn(buildUpTime, easeOutTime, amplitude, frequency, Vector3.zero);
        }

        private void Update() {
            if (Timer < _buildUpTime) {
                _currentAmp = Mathf.Lerp(0, _amplitude, Timer / _buildUpTime);
                _currentFreq = Mathf.Lerp(0, _frequency, Timer / _buildUpTime);
            } else if (Timer > EaseOutPoint) {
                _currentAmp = Mathf.Lerp(_amplitude, 0, Timer - EaseOutPoint / _easeOutTime);
                _currentFreq = Mathf.Lerp(_frequency, 0, Timer - EaseOutPoint / _easeOutTime);
            } else {
                _currentAmp = _amplitude;
                _currentFreq = _frequency;
            }

            ScreenShakeReference.SetNoise(_offset, _currentAmp, _currentFreq);
        }

        protected override void OnEnd() {
            ScreenShakeReference.SetNoise(Vector3.zero, 0, 0);
        }
    }
}
