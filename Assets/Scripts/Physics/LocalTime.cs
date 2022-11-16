using System.Collections.Generic;
using UnityEngine;

//#define USE_LOCAL_TIME_CACHE

namespace Physics {
    public class LocalTime {
        private static readonly Collider2D[] _hits = new Collider2D[8]; // To avoid GC pressure

        private static readonly int _layerMask = LayerMask.GetMask("Local Time");

        private static readonly Dictionary<Vector2Int, float> _multiplierAtCache = new();

        /// <summary>
        ///     Returns the time multiplier for a given position by querying collisions with LocalTimeProviders.
        /// </summary>
        public static float MultiplierAt(Vector2Int position) {
#if USE_LOCAL_TIME_CACHE
            // Check cache
            if (_multiplierAtCache.ContainsKey(position)) {
                return _multiplierAtCache[position];
            }
#endif

            var time = 1.0f;
            var count = Physics2D.OverlapPointNonAlloc(position, _hits, _layerMask);

            for (var i = 0; i < count; i++) {
                var provider = _hits[i].GetComponent<LocalTimeProvider>();

                if (provider == null) {
                    Debug.LogWarning(
                        "Found a collider on the 'Local Time' layer without a LocalTimeProvider component.");
                    continue;
                }

                time *= provider.TimeMultiplier;
            }

#if USE_LOCAL_TIME_CACHE
            // Cache result
            _multiplierAtCache.Add(position, time);
#endif

            return time;
        }

        public static float MultiplierAt(Vector3 position) {
            var tilePos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            return MultiplierAt(tilePos);
        }

        public static void InvalidateMultiplierAtCache() {
            _multiplierAtCache.Clear();
        }

        public static float DeltaTimeAt(Vector3 position) {
            return Time.deltaTime * MultiplierAt(position);
        }

        public static float DeltaTimeAt(Transform transform) {
            return DeltaTimeAt(transform.position);
        }

        public static float DeltaTimeAt(GameObject obj) {
            return DeltaTimeAt(obj.transform);
        }

        public static float DeltaTimeAt(MonoBehaviour obj) {
            return DeltaTimeAt(obj.transform);
        }
    }
}
