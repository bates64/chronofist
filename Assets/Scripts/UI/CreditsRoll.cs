using UnityEngine;

namespace Ui {
    public class CreditsRoll : MonoBehaviour {
        public float scrollSpeed = 0.5f;
        public float totalDistance = 1000f;

        private float distanceTraveled;
        private Vector3 startPos;
        public bool IsDone => distanceTraveled > totalDistance;

        public void Start() {
            startPos = transform.position;
        }

        private void Update() {
            var multiplier = 1f;
            if (InputManager.InterfaceInput.interact) multiplier = 4f;
            var advance = scrollSpeed * Time.deltaTime * multiplier;

            transform.Translate(Vector3.up * advance);
            distanceTraveled += advance;
        }

        public void ResetCredits() {
            distanceTraveled = 0;
            transform.localPosition = startPos;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * totalDistance);
        }
    }
}
