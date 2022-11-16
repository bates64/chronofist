using UnityEngine;

namespace UI.HUD.HealthBar {
    public class HeartContainer : MonoBehaviour {
        public int healthValue;

        private HealthBarSection _parentSection;
        private SpriteRenderer _spriteRenderer;

        private void Awake() {
            _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        private void Update() {
            if (_parentSection != null) {
                if (_parentSection.health.health >= healthValue)
                    _spriteRenderer.sprite = _parentSection.fullSprite;
                else
                    _spriteRenderer.sprite = _parentSection.emptySprite;

                _spriteRenderer.enabled = _parentSection.health.maxHealth >= healthValue;
            }
        }

        public void SetParentSection(HealthBarSection parentSection) {
            _parentSection = parentSection;
        }
    }
}
