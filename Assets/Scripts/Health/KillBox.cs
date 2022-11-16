using UnityEngine;

namespace Health {
    [RequireComponent(typeof(Collider2D))]
    public class KillBox : MonoBehaviour {
        public void Start() {
            // Verify we have a collider with isTrigger=true.
            var collider = GetComponent<Collider2D>();
            if (collider == null)
                Debug.LogError("KillBox must have a Collider2D component.");
            else if (!collider.isTrigger) Debug.LogError("KillBox must have a Collider2D with isTrigger=true.");

            // Verify we're not on the Level layer (we shouldn't collide with the Controller2D).
            if (gameObject.layer == LayerMask.NameToLayer("Level"))
                Debug.LogError("KillBox must not be on the Level layer.");
        }

        // Note: for this method to work, the other object must have a RigidBody2D.
        public void OnTriggerEnter2D(Collider2D other) {
            var health = other.GetComponent<Health>();

            if (health != null) health.Kill();
        }
    }
}
