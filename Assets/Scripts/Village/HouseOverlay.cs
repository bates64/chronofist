using UnityEngine;

namespace Village {
    [RequireComponent(typeof(SpriteRenderer))]
    public class HouseOverlay : MonoBehaviour {
        public float visibleWhenXLessThan;
        public GameObject triggerer;

        private SpriteRenderer spriteRenderer;

        private void Start() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update() {
            spriteRenderer.enabled = triggerer.transform.position.x < visibleWhenXLessThan;
        }
    }
}
