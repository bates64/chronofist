using UnityEngine;

namespace Combat {
    /// <summary>
    ///   This component changes the color of a child SpriteRenderer depending on the Enemy component state:
    ///     - Idle: white
    ///     - Invulnerable / taking damage: red
    ///     - Dead: black
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    public class DebugEnemyStateAsColor : MonoBehaviour {
        private Enemy enemy;
        private SpriteRenderer spriteRenderer;

        private void Awake() {
            enemy = GetComponent<Enemy>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update() {
            if (!spriteRenderer || !enemy) return;

            if (!enemy.IsAlive()) {
                spriteRenderer.color = Color.black;
            } else if (enemy.IsVulnerable()) {
                spriteRenderer.color = Color.white;
            } else {
                spriteRenderer.color = Color.red;
            }
        }
    }
}
