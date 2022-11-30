using Physics;
using UnityEngine;

namespace Ui {
    public class PauseUi : MonoBehaviour {
        public float speed = 1f;
        public LocalTimeProvider globalTimeProvider;

        private void Update() {
            var isActive = InputManager.CurrentMode == InputManager.Mode.Interface;

            globalTimeProvider.TimeMultiplier = Mathf.Lerp(globalTimeProvider.TimeMultiplier, isActive ? 0 : 1,
                Time.deltaTime * speed);

            var targetY = isActive ? 0f : -36f;
            transform.localPosition = new Vector3(transform.localPosition.x,
                Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * speed), transform.localPosition.z);
        }
    }
}
