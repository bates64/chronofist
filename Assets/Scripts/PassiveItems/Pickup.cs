using System.Collections.Generic;
using UnityEngine;

namespace PassiveItems {
    public abstract class Pickup : MonoBehaviour {
        public static readonly List<Pickup> ActivePickups = new();
        [SerializeField] private float radius = 1;
        protected SpriteRenderer SpriteRenderer;

        public float Radius => radius;

        protected virtual void Awake() {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnEnable() {
            ActivePickups.Add(this);
        }

        private void OnDisable() {
            ActivePickups.Remove(this);
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public void PickUp(Pickupper pickupper) {
            ActivePickups.Remove(this);
            OnPickup(pickupper);
        }

        protected abstract void OnPickup(Pickupper pickupper);
    }
}
