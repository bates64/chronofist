using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Combat {
    public class HitboxManager : MonoBehaviour {
        private List<CircleCollider2D> colliders = new List<CircleCollider2D>();

        private void Awake() {
            colliders = GetComponentsInChildren<CircleCollider2D>().ToList();
        }

        public void Restore() {
            foreach (var col in colliders) {
                col.radius = 1;
                col.enabled = false;
                col.offset = Vector2.zero;
                col.transform.localPosition = Vector2.zero;
            }
        }
    }
}
