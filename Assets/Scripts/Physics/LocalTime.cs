using UnityEngine;

namespace Physics {
    public class LocalTime {
        static private Collider2D[] _hits = new Collider2D[8]; // To avoid GC pressure

        static private int _layerMask = LayerMask.GetMask("Local Time");

        /// <summary>
        /// Returns the time multiplier for a given position by querying collisions with LocalTimeProviders.
        /// </summary>
        static public float multiplierAt(Vector3 position) {
            // PERF: we could potentially memoize this calculation to avoid the physics query

            float time = 1.0f;
            int count = Physics2D.OverlapPointNonAlloc(position, _hits, _layerMask);

            Debug.Log(count);

            for (int i = 0; i < count; i++) {
                var provider = _hits[i].GetComponent<LocalTimeProvider>();

                if (provider == null) {
                    Debug.LogWarning("Found a collider on the 'Local Time' layer without a LocalTimeProvider component.");
                    continue;
                }

                time *= provider.timeMultiplier;
            }

            return time;
        }

        static public float deltaTimeAt(Vector3 position) {
            return Time.deltaTime * multiplierAt(position);
        }
    }
}
