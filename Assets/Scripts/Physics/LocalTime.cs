using System.Collections.Generic;
using UnityEngine;

//#define USE_LOCAL_TIME_CACHE

namespace Physics {
    public class LocalTime {
        static private Collider2D[] _hits = new Collider2D[8]; // To avoid GC pressure

        static private int _layerMask = LayerMask.GetMask("Local Time");

        static private Dictionary<Vector2Int, float> _multiplierAtCache = new Dictionary<Vector2Int, float>();

        /// <summary>
        /// Returns the time multiplier for a given position by querying collisions with LocalTimeProviders.
        /// </summary>
        static public float MultiplierAt(Vector2Int position) {
#if USE_LOCAL_TIME_CACHE
            // Check cache
            if (_multiplierAtCache.ContainsKey(position)) {
                return _multiplierAtCache[position];
            }
#endif

            float time = 1.0f;
            int count = Physics2D.OverlapPointNonAlloc(position, _hits, _layerMask);

            for (int i = 0; i < count; i++) {
                var provider = _hits[i].GetComponent<LocalTimeProvider>();

                if (provider == null) {
                    Debug.LogWarning("Found a collider on the 'Local Time' layer without a LocalTimeProvider component.");
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

        static public float MultiplierAt(Vector3 position) {
            Vector2Int tilePos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            return MultiplierAt(tilePos);
        }

        static public void InvalidateMultiplierAtCache() {
            _multiplierAtCache.Clear();
        }

        static public float DeltaTimeAt(Vector3 position) {
            return Time.deltaTime * MultiplierAt(position);
        }

        static public float DeltaTimeAt(Transform transform) {
            return DeltaTimeAt(transform.position);
        }

        static public float DeltaTimeAt(GameObject obj) {
            return DeltaTimeAt(obj.transform);
        }

        static public float DeltaTimeAt(MonoBehaviour obj) {
            return DeltaTimeAt(obj.transform);
        }
    }
}
