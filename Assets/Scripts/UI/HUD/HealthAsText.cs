using System;
using UnityEngine;

namespace Ui.HUD {
    [ExecuteAlways]
    [RequireComponent(typeof(GameText))]
    public class HealthAsText : MonoBehaviour {
        public enum HealthTextFormat {
            Fraction,
            Percent,
            Time
        };

        public Health.Health health;
        public HealthTextFormat format = HealthTextFormat.Fraction;

        private GameText text;

        private void Awake() {
            text = GetComponent<GameText>();
            if (text == null) {
                Debug.LogError("HealthAsText: No GameText component found on this object");
            }

            if (health == null) health = GetComponent<Health.Health>();
            if (health == null) health = GetComponentInParent<Health.Health>();

            if (health != null) {
                health.OnChange += UpdateText;
            } else {
                Debug.LogError("HealthAsText: No health component found on this or parent object");
            }
        }

        private void UpdateText(float hp, float maxHp) {
            UpdateText();
        }

        private void UpdateText() {
            if (text == null) {
                return;
            }
            if (health == null) {
                text.Text = "";
                return;
            }

            text.Text = format switch {
                HealthTextFormat.Fraction => health.ToFractionString(),
                HealthTextFormat.Percent => health.ToPercentString(),
                HealthTextFormat.Time => health.ToTimeString(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
