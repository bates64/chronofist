// Based on https://github.com/Unity-Technologies/com.unity.cinemachine/blob/master/com.unity.cinemachine/Runtime/Behaviours/CinemachinePixelPerfect.cs

using UnityEngine;
using Cinemachine;

namespace World
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class CustomCinemachinePixelPerfect : CinemachineExtension
    {
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            // This must run during the Body stage because CinemachineConfiner also runs during Body stage,
            // and CinemachinePixelPerfect needs to run before CinemachineConfiner as the confiner reads the
            // orthographic size. We also altered the script execution order to ensure this.
            if (stage != CinemachineCore.Stage.Body)
                return;

            var brain = CinemachineCore.Instance.FindPotentialTargetBrain(vcam);
            if (brain == null || !brain.IsLive(vcam))
                return;

            UnityEngine.U2D.PixelPerfectCamera pixelPerfectCamera;
            brain.TryGetComponent(out pixelPerfectCamera);
            if (pixelPerfectCamera == null || !pixelPerfectCamera.isActiveAndEnabled)
                return;

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && !pixelPerfectCamera.runInEditMode)
                return;
#endif

            var lens = state.Lens;
            lens.OrthographicSize = 13.5f;//pixelPerfectCamera.CorrectCinemachineOrthoSize(lens.OrthographicSize);
            Debug.Log("OrthographicSize: " + lens.OrthographicSize);
            state.Lens = lens;
        }
    }
}
