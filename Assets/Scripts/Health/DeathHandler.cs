using UnityEngine;

namespace Health {
    [RequireComponent(typeof(Health))]
    public abstract class DeathHandler : MonoBehaviour {
        public void Start() {
            var health = GetComponent<Health>();
            health.OnDeath += OnDeath;
        }

        public abstract void OnDeath();
    }
}
