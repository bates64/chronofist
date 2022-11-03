using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics {
    public class LocalTimeProvider : MonoBehaviour {
        public float timeMultiplier = 1.0f;

        void Start() {
            if (gameObject.layer != LayerMask.NameToLayer("Local Time")) {
                Debug.LogWarning("LocalTimeProvider should be on the 'Local Time' layer.");
            }
        }

        void Update() {
            // Nothing to do!
            // See LocalTime.multiplierAt for the actual logic.
        }
    }
}
