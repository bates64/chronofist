using System.Collections.Generic;
using UnityEngine;
using Physics;

namespace Enemy {
    [RequireComponent(typeof(Controller2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BaseEnemy : MonoBehaviour {
        protected float gravity = 4f;
        protected Vector2 gravityVelocity = Vector2.zero;

        protected Vector2 knockbackVelocity = Vector2.zero;
        protected Vector2 knockbackDamping = new Vector2(0.6f, 0.6f);

        protected Controller2D controller;
        protected Health.Health health; // May be null

        private float damageCooldown = 0f;

        public static List<BaseEnemy> enemies = new List<BaseEnemy>();

        protected virtual void Awake() {
            controller = GetComponent<Controller2D>();
            health = GetComponent<Health.Health>();
            enemies.Add(this);
        }

        protected virtual void Destroy() {
            enemies.Remove(this);
        }

        protected virtual void Update() {
            var deltaTime = LocalTime.DeltaTimeAt(this);

            // Gravity
            gravityVelocity.y -= gravity * deltaTime;
            controller.Move(gravityVelocity * deltaTime);
            if (controller.isGrounded)
                gravityVelocity.y = -gravity;

            // Knockback
            controller.Move(knockbackVelocity * deltaTime);
            knockbackVelocity -= knockbackVelocity * knockbackDamping * deltaTime;
            // Bounce off walls/floors/ceilings
            /*if (controller.collision.left)
                knockbackVelocity.x *= -1;
            if (controller.collision.right)
                knockbackVelocity.x *= -1;
            if (controller.collision.down)
                knockbackVelocity.y *= -1;
            if (controller.collision.up)
                knockbackVelocity.y *= -1;*/

            // Timers
            damageCooldown -= deltaTime;
        }

        public virtual void ApplyKnockback(Vector2 knockback) {
            knockbackVelocity += knockback;
            gravityVelocity = Vector2.zero;
        }

        public bool TryResetCooldown(float cooldown = 0f) {
            if (damageCooldown > 0f) {
                return false;
            } else {
                damageCooldown = cooldown;
                return true;
            }
        }

        public Health.Health GetHealth() {
            return health;
        }
    }
}
