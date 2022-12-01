using System;
using General;
using Physics;
using UnityEngine;

namespace Combat {
    [RequireComponent(typeof(Controller2D)), RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour {
        public float gravity = 4f;
        public Vector2 knockbackDamping = new(0.6f, 0.6f); // Higher = heavier
        public float walkSpeed = 4f;

        [HideInInspector] public Controller2D Controller;
        public Health.Health health; // May be null
        private float damageCooldown;
        protected Vector2 gravityVelocity = Vector2.zero;
        private Vector2 knockbackVelocity = Vector2.zero;
        private float facingDirection = 1f;
        private LayerMask playerAttackLayer;

        private static Transform _playerTransform;

        public static readonly int StateId = Animator.StringToHash("StateId");
        public static readonly int Play = Animator.StringToHash("Play");
        public static readonly int DamageS = Animator.StringToHash("Damage");

        [NonSerialized] public Vector3 HomePosition;

        public static Vector2 PlayerPosition
        {
            get
            {
                if (_playerTransform is null)
                {
                    _playerTransform = FindObjectOfType<Player>().transform;
                }
                return _playerTransform.position;
            }
        }
        public float DeltaTime => LocalTime.DeltaTimeAt(this);

         protected virtual void Awake() {
            HomePosition = transform.position;
            Controller = GetComponent<Controller2D>();
            health = GetComponent<Health.Health>();
            playerAttackLayer = LayerMask.NameToLayer("PlayerAttack");
        }

        protected virtual void Update()
        {
            var deltaTime = DeltaTime;
            Gravity(deltaTime);
            Knockback(deltaTime);
            Cooldowns(deltaTime);
        }

        public void Flip(int dir,Transform MyEnemyTransform)
        {
            var transform = MyEnemyTransform;
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * -dir;
            transform.localScale = newScale;
        }

        protected virtual void Cooldowns(float deltaTime)
        {
            // Timers
            damageCooldown -= deltaTime;
        }

        protected void Gravity(float deltaTime)
        {
            // Gravity
            gravityVelocity.y -= gravity * deltaTime;
            Controller.Move(gravityVelocity * deltaTime);
            if (Controller.isGrounded)
            {
                gravityVelocity.y = -gravity;
            }
        }

        protected virtual void Knockback(float deltaTime)
        {
            // Knockback
            Controller.Move(knockbackVelocity * deltaTime);
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
        }

        private void OnTriggerEnter2D(Collider2D col) {
            Damage(col);
        }

        protected void Damage(Collider2D col)
        {
            // Check for incoming damage...
            if (col.gameObject.layer == playerAttackLayer) {
                var player = Player.Instance;
                if (TryResetCooldown(0.05f)) {
                    WhenDamage();
                    player.DidDamageEnemy(this);
                    ApplyKnockback(player.AttackKnockback);
                    if (health) health.ApplyDamage(player.AttackDamage);
                }
            }
        }

        protected virtual void WhenDamage()
        {

        }

        public void Walk(float direction) {
            facingDirection = direction;
            Controller.Move(walkSpeed * direction * LocalTime.DeltaTimeAt(this) * Vector2.right);
        }

        public void Walk() {
            Walk(facingDirection);
        }

        public float FacingDirection() {
            return facingDirection;
        }

        public void ApplyKnockback(Vector2 knockback)
        {
            WhenKnockBackApplied(knockback);
            knockbackVelocity += knockback;
            gravityVelocity = Vector2.zero;
        }

        protected virtual void WhenKnockBackApplied(Vector2 knockback)
        {

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
            return IsAlive() && damageCooldown <= 0f;
        }

        public float DistanceFromHome() {
            return Vector3.Distance(HomePosition, transform.position);
        }

        public bool IsGrounded() {
            return Controller.isGrounded;
        }
    }
}
