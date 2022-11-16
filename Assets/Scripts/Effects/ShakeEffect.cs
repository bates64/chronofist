using System;
using Combat;
using UnityEngine;

namespace Effects
{
    public class ShakeEffect : Effect
    {
        private Vector3 _offset;
        private float _amplitude;
        private float _frequency;
        private float _buildUpTime;
        private float _easeOutTime;
        private float _duration;

        private float _currentAmp;
        private float _currentFreq;

        private float Timer => _duration - TimeToLive;
        private float EaseOutPoint => _duration - _easeOutTime;

        public ShakeEffect SetUp(Vector3 offset,float amplitude, float frequency,float duration,float buildUpTime, float easeOutTime)
        {
            _offset = offset;
            _amplitude = amplitude;
            _frequency = frequency;
            TimeToLive = duration;
            _duration = duration;
            _buildUpTime = buildUpTime;
            _easeOutTime = easeOutTime;
            _buildUpTime = Mathf.Clamp(_buildUpTime, 0, duration);
            _easeOutTime = Mathf.Clamp(_easeOutTime, _buildUpTime, duration);
            return this;
        }

        public static void Shake(Vector3 offset,float amplitude, float frequency,float duration,float buildUpTime, float easeOutTime)
        {
            var effectObject = new GameObject();
            var effect = effectObject.AddComponent<ShakeEffect>();
            effect.SetUp(offset,amplitude,frequency,duration,buildUpTime,easeOutTime);
        }

        private void Update()
        {
            if (Timer < _buildUpTime)
            {
                _currentAmp = Mathf.Lerp(0, _amplitude, Timer / _buildUpTime);
                _currentFreq = Mathf.Lerp(0, _frequency, Timer / _buildUpTime);
            }
            else if (Timer > EaseOutPoint)
            {
                _currentAmp = Mathf.Lerp(_amplitude, 0, Timer - EaseOutPoint / _easeOutTime);
                _currentFreq = Mathf.Lerp(_frequency, 0, Timer - EaseOutPoint / _easeOutTime);
            }
            else
            {
                _currentAmp = _amplitude;
                _currentFreq = _frequency;
            }
            ScreenShakeReference.SetNoise(_offset,_currentAmp,_currentFreq);
        }

        protected override void OnEnd()
        {
            ScreenShakeReference.SetNoise(Vector3.zero,0,0);
        }
    }
}
