using UnityEngine;
using Physics;

namespace Effects {
    public class TimeEffect : Effect {
        public float TimeMultiplier = 0f;
        protected GameObject Obj;
        protected LocalTimeProvider LocalTimeProvider;
        protected BoxCollider2D Collider;

        public static GameObject Spawn(float duration, float timeMultiplier) {
            var obj = new GameObject("TimeEffect");
            var effect = obj.AddComponent<TimeEffect>();
            effect.TimeToLive = duration;
            effect.TimeMultiplier = timeMultiplier;
            return obj;
        }

        public void Start() {
            Obj = new GameObject("TimeEffect's LocalTimeProvider");
            Obj.layer = LocalTimeProvider.Layer;

            LocalTimeProvider = Obj.AddComponent<LocalTimeProvider>();
            LocalTimeProvider.TimeMultiplier = TimeMultiplier;

            Collider = Obj.AddComponent<BoxCollider2D>();
            Collider.size = new Vector2(99999f, 99999f);
        }

        public void Update() {
            LocalTimeProvider.TimeMultiplier = TimeMultiplier;
        }

        public void OnDestroy() {
            Destroy(Obj);
        }
    }
}
