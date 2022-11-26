using UnityEngine;
using Physics;

namespace Health {
    [RequireComponent(typeof(Health))]
    public class HealthTimeDepletion : MonoBehaviour {
        /// <summary>
        ///     The amount of health to remove per game second.
        /// </summary>
        public float depletionRate = 1f;

        private Health health;

        private void Awake() {
            health = GetComponent<Health>();
        }

        private void Update() {
            health.ApplyDamage(depletionRate * LocalTime.DeltaTimeAt(this), true);
        }
    }
}
