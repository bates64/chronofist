using System.Collections.Generic;
using Physics;
using UnityEngine;

namespace Enemy {
    [RequireComponent(typeof(Controller2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BaseEnemy : MonoBehaviour {
        public static List<BaseEnemy> enemies = new();

        protected Controller2D controller;

        private float damageCooldown;
        protected float gravity = 4f;
        protected Vector2 gravityVelocity = Vector2.zero;
        protected Health.Health health; // May be null
        protected Vector2 knockbackDamping = new(0.6f, 0.6f);

        protected Vector2 knockbackVelocity = Vector2.zero;

        protected virtual void Awake() {
            controller = GetComponent<Controller2D>();
            health = GetComponent<Health.Health>();
            enemies.Add(this);
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

        protected virtual void Destroy() {
            enemies.Remove(this);
        }

        public virtual void ApplyKnockback(Vector2 knockback) {
            knockbackVelocity += knockback;
            gravityVelocity = Vector2.zero;
        }

        public bool TryResetCooldown(float cooldown = 0f) {
            if (damageCooldown > 0f) {
                return false;
            }

            damageCooldown = cooldown;
            return true;
        }

        public Health.Health GetHealth() {
            return health;
        }
    }
}
