using UnityEngine;
using General;

namespace Health {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Health : MonoBehaviour {
        public float health = 16f;
        public float maxHealth = 16f;

        public bool isDead => health <= 0f;

        public event Util.DFloat OnTakeDamage;
        public event Util.DFloat OnHeal;
        public event Util.DVoid OnFullHealth;
        public event Util.DVoid OnDeath;

        public event DChange OnChange;
        public delegate void DChange(float health, float maxHealth);

        public void Start() {
            if (GetComponent<DeathHandler>() == null) {
                Debug.LogWarning($"'{gameObject.name}' has Health but no DeathHandler");
            }

            // TODO: if maxHealth changes, check health & invoke OnChange
        }

        public void ApplyDamage(float damage) {
            if (isDead) {
                return;
            }

            health -= damage;
            OnTakeDamage?.Invoke(damage);
            OnChange?.Invoke(health, maxHealth);

            if (health <= 0f) {
                health = 0f;
                OnDeath?.Invoke();
            }
        }

        public void Kill() {
            ApplyDamage(health);
        }

        public void Heal(float heal, bool allowRevive) {
            if (isDead && !allowRevive) {
                return;
            }

            health += heal;
            OnHeal?.Invoke(heal);
            OnChange?.Invoke(health, maxHealth);

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
