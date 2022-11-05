using UnityEngine;

namespace World {
    /// <summary>
    /// Activates levels when entering their bounds and deactivates them when leaving.
    /// There can only be one LevelActivator in the scene - put it on the player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class LevelActivator : MonoBehaviour {
        private static LevelActivator _instance;

        private void Awake() {
            if (_instance != null) {
                Debug.LogError("There can only be one LevelActivator (the player)!");
                Destroy(this);
                return;
            }

            _instance = this;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("LevelBounds")) {
                LevelManager.EnterLevel(other.gameObject);
            }
        }

        public static Transform Transform() {
            return _instance.gameObject.transform;
        }
    }
}
