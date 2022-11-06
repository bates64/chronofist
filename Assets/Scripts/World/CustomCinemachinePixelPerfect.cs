using Cinemachine;

namespace World {
    public class CustomCinemachinePixelPerfect : CinemachinePixelPerfect {
        public static float ORTHO_SIZE = 16.875f;

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime
        ) {
            base.PostPipelineStageCallback(vcam, stage, ref state, deltaTime);
            state.Lens.OrthographicSize = ORTHO_SIZE;
        }
    }
}
