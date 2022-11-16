using UnityEngine;

namespace PassiveItems {
    public class Pickupper : MonoBehaviour {
        public float radius = 1.0f;

        private void Update() {
            foreach (var pickup in Pickup.ActivePickups)
                if (Vector2.Distance(transform.position, pickup.transform.position) < radius)
                    pickup.PickUp(this);
        }
    }
}
