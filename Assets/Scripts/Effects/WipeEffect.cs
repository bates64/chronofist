using UnityEngine;

namespace Effects {
    public class WipeEffect : Effect {
        private float duration;

        public void Start() {
            duration = TimeToLive;
        }

        public void Update() {
            // Animate y position from -35 to 35
            transform.localPosition = new Vector3(0, Mathf.SmoothStep(-35, 35, 1 - (TimeToLive / duration)), 1);
        }

        public static GameObject Spawn(float duration) {
            var obj = new GameObject("WipeEffect");

            if (Camera.main) obj.transform.parent = Camera.main.transform;

            var effect = obj.AddComponent<WipeEffect>();
            effect.TimeToLive = duration;

            var sprite = obj.AddComponent<SpriteRenderer>();
            sprite.sprite = Resources.Load<Sprite>("Wipe");

            return obj;
        }
    }
}
