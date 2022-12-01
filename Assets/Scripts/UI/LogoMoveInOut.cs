using UnityEngine;

namespace Ui {
    public class LogoMoveInOut : MonoBehaviour {
        public bool showLogo;
        public float lerpAlpha = 0.1f;

        private float startX;

        private void Start() {
            startX = transform.localPosition.x;
        }

        private void Update() {
            var targetX = showLogo ? 0 : startX;
            var x = Mathf.Lerp(transform.localPosition.x, targetX, lerpAlpha);
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
