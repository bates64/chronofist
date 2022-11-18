using System;
using Physics;
using UnityEngine;

namespace Combat {
    [RequireComponent(typeof(Controller2D)), RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour {
        public float gravity = 4f;
        public Vector2 knockbackDamping = new(0.6f, 0.6f); // Higher = heavier
        public float walkSpeed = 4f;

        private Controller2D controller;
        private Health.Health health; // May be null
        private float damageCooldown;
        private Vector2 gravityVelocity = Vector2.zero;
        private Vector2 knockbackVelocity = Vector2.zero;
        private float facingDirection = 1f;
        private LayerMask playerAttackLayer;

        [NonSerialized] public Vector3 HomePosition;

        private void Awake() {
            HomePosition = transform.position;
            controller = GetComponent<Controller2D>();
            health = GetComponent<Health.Health>();
            playerAttackLayer = LayerMask.NameToLayer("PlayerAttack");
        }

        private void Update() {
            var deltaTime = LocalTime.DeltaTimeAt(this);

            // Gravity
            gravityVelocity.y -= gravity * deltaTime;
            controller.Move(gravityVelocity * deltaTime);
            if (controller.isGrounded) {
                gravityVelocity.y = -gravity;
            }

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

        private void OnTriggerEnter2D(Collider2D col) {
            // Check for incoming damage...
            if (col.gameObject.layer == playerAttackLayer) {
                var player = Player.Instance;
                if (TryResetCooldown(0.05f)) {
                    player.DidDamageEnemy(this);
                    ApplyKnockback(player.AttackKnockback);
                    if (health) health.ApplyDamage(player.AttackDamage);
                }
            }
        }

        public void Walk(float direction) {
            facingDirection = direction;
            controller.Move(walkSpeed * direction * LocalTime.DeltaTimeAt(this) * Vector2.right);
        }

        public void Walk() {
            Walk(facingDirection);
        }

        public float FacingDirection() {
            return facingDirection;
        }

        public void ApplyKnockback(Vector2 knockback) {
            knockbackVelocity += knockback;
            gravityVelocity = Vector2.zero;
        }

        public bool TryResetCooldown(float cooldown = 0f) {
            if (!IsVulnerable()) {
                return false;
            }

            damageCooldown = cooldown;
            return true;
        }

        public bool IsAlive() {
            if (health) {
                return !health.isDead;
            } else {
                return true;
            }
        }

        public bool IsVulnerable() {
            return IsAlive() && damageCooldown < 0f;
        }

        public float DistanceFromHome() {
            return Vector3.Distance(HomePosition, transform.position);
        }

        public bool IsGrounded() {
            return controller.isGrounded;
        }
    }
}
