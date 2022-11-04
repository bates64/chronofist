using UnityEngine;
using General;

namespace Health {
    public class Health : MonoBehaviour {
        public float health = 100f;
        public float maxHealth = 100f;
        public float minHealth = 0f;

        public bool isDead => health <= minHealth;

        public event Util.DFloat OnTakeDamage;
        public event Util.DFloat OnHeal;
        public event Util.DVoid OnFullHealth;
        public event Util.DVoid OnDeath;

        public void ApplyDamage(float damage) {
            health -= damage;
            OnTakeDamage?.Invoke(damage);

            if (health <= minHealth) {
                health = minHealth;
                OnDeath?.Invoke();
            }
        }

        public void Heal(float heal) {
            health += heal;
            OnHeal?.Invoke(heal);

            if (health >= maxHealth) {
                health = maxHealth;
                OnFullHealth?.Invoke();
            }
        }

        public void OnGUI() {
            var screenPos = Camera.main.WorldToScreenPoint(transform.position);

            GUI.Label(
                new Rect(screenPos.x, Screen.height - screenPos.y, 0, 0),
                $"{health}/{maxHealth}",
                new GUIStyle() {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter,
                    normal = new GUIStyleState() {
                        textColor = isDead ? Color.red : Color.green
                    }
                }
            );
        }
    }
}
