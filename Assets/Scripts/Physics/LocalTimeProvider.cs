using UnityEngine;

namespace Physics {
    public class LocalTimeProvider : MonoBehaviour {
        public float TimeMultiplier = 1.0f;

        public static int Layer => LayerMask.NameToLayer("Local Time");

        void Start() {
            if (gameObject.layer != Layer) {
                Debug.LogWarning("LocalTimeProvider should be on the 'Local Time' layer.");
            }

            LocalTime.InvalidateMultiplierAtCache();
        }

        void Update() {
            if (transform.hasChanged) {
                LocalTime.InvalidateMultiplierAtCache();
                transform.hasChanged = false;
            }
        }

        void Destroy() {
            LocalTime.InvalidateMultiplierAtCache();
        }
    }
}
