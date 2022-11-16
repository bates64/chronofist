using General;
using UnityEngine;

namespace UI.HUD.HealthBar {
    public class HealthBarSection : MonoBehaviour {
        public int minHealth = 1;
        public int numContainers = 8;

        public Sprite emptySprite;
        public Sprite fullSprite;

        public Health.Health health;

        private void Awake() {
            createContainers();
        }

        private void createContainers() {
            for (var i = 0; i < numContainers; i++) {
                var healthValue = minHealth + i;

                var obj = new GameObject($"HeartContainer {healthValue}");
                obj.transform.SetParent(transform);
                obj.transform.localPosition =
                    new Vector3(i * 10f * Util.PIXEL, i % 2 == 0 ? -Util.PIXEL : Util.PIXEL, 0f);

                var hc = obj.AddComponent<HeartContainer>();
                hc.healthValue = healthValue;
                hc.SetParentSection(this);
            }
        }
    }
}
