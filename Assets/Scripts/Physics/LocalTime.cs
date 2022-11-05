using System.Collections.Generic;
using UnityEngine;

namespace Physics {
    public class LocalTime {
        static private Collider2D[] _hits = new Collider2D[8]; // To avoid GC pressure

        static private int _layerMask = LayerMask.GetMask("Local Time");

        static private Dictionary<Vector2Int, float> _multiplierAtCache = new Dictionary<Vector2Int, float>();

        /// <summary>
        /// Returns the time multiplier for a given position by querying collisions with LocalTimeProviders.
        /// </summary>
        static public float multiplierAt(Vector2Int position) {
            // Check cache
            if (_multiplierAtCache.ContainsKey(position)) {
                return _multiplierAtCache[position];
            }

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

            // Cache result
            _multiplierAtCache.Add(position, time);

            return time;
        }

        static public float multiplierAt(Vector3 position) {
            Vector2Int tilePos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            return multiplierAt(tilePos);
        }

        static public void InvalidateMultiplierAtCache() {
            _multiplierAtCache.Clear();
        }

        static public float deltaTimeAt(Vector3 position) {
            return Time.deltaTime * multiplierAt(position);
        }
    }
}
