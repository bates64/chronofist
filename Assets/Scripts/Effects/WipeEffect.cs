using UnityEngine;

namespace Effects {
    public class WipeEffect : Effect {
        private float duration;
        public float Direction = 1f;

        public void Start() {
            duration = TimeToLive;
        }

        public void Update() {
            // Animate y position
            transform.localPosition = new Vector3(0, Mathf.SmoothStep(-35 * Direction, 35 * Direction, 1 - (TimeToLive / duration)), 1);
        }

        public static GameObject Spawn(float duration, float direction = 1f) {
            var obj = new GameObject("WipeEffect");

            if (Camera.main) obj.transform.parent = Camera.main.transform;

            var effect = obj.AddComponent<WipeEffect>();
            effect.TimeToLive = duration;
            effect.Direction = direction;

            var sprite = obj.AddComponent<SpriteRenderer>();
            sprite.sprite = Resources.Load<Sprite>("Wipe");

            return obj;
        }
    }
}
