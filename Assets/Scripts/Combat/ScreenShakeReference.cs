using System;
using System.Collections.Generic;
using Cinemachine;
using Effects;
using General;
using UnityEngine;

namespace Combat
{
    public class ScreenShakeReference : Singleton<ScreenShakeReference>
    {
        private CinemachineBasicMultiChannelPerlin _currentPerlin;
        private CinemachineVirtualCamera _currentCamera;

        protected override void init() {}

        public static void SetCurrentCamera(CinemachineVirtualCamera camera)
        {
            if (Instance._currentPerlin)
            {
                Instance._currentPerlin.m_AmplitudeGain = 0;
                Instance._currentPerlin.m_FrequencyGain = 0;
                Instance._currentPerlin.m_PivotOffset = Vector3.zero;
            }
            Instance._currentCamera = camera;
            Instance._currentPerlin = Instance._currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public static void SetNoise(Vector3 offset,float amplitude, float frequency)
        {
            if (Instance._currentPerlin is null) return;
            Instance._currentPerlin.m_AmplitudeGain = amplitude;
            Instance._currentPerlin.m_FrequencyGain = frequency;
            Instance._currentPerlin.m_PivotOffset = offset;
        }

    }
}
