using System;
using UI;
using UnityEngine;
using Effects;

namespace Physics {
    [RequireComponent(typeof(Controller2D)), RequireComponent(typeof(Health.Health))]
    public class Player : MonoBehaviour {
        public static Player Instance { get; private set; }

        private Controller2D controller;

        private void Awake() {
            controller = GetComponent<Controller2D>();
            controller.collision.OnWallBump += OnWallBump;
            controller.collision.OnLanding += OnLanding;
            controller.collision.OnCeilingBump += OnCeilingBump;

            InputManager.PlayerInput.OnJumpChange += OnInputJump;
            InputManager.PlayerInput.OnAttackChange += OnInputAttack;
            InputManager.PlayerInput.OnSpecialChange += OnInputSpecial;
            InputManager.PlayerInput.OnDashChange += OnInputDash;

            ReplenishAirAttacks();

            Instance = this;
        }

        private void Update() {
            var deltaTime = LocalTime.DeltaTimeAt(this);

            timeSinceStoredJump += deltaTime;
            physicsDisableTime = Mathf.Max(0f, physicsDisableTime - deltaTime);
            attackDisableTime = Mathf.Max(0f, attackDisableTime - deltaTime);
            attackAnimTime = Mathf.Max(0f, attackAnimTime - deltaTime);

            if (physicsDisableTime <= 0f) {
                UpdateMoveVelocity(InputManager.PlayerInput.Movement.x);
                ApplyGravity();
            }

            UpdateWalls();
            UpdateAttacks();

            var totalVelocity = new Vector2(moveVelocity, yVelocity) + jumpVelocity;
            controller.Move(totalVelocity * LocalTime.DeltaTimeAt(this));

            UiManager.DebugUi.SetStateName(
                $"attackType: {attackType} attackAnimTime: {attackAnimTime} attackDisableTime: {attackDisableTime} physicsDisableTime: {physicsDisableTime}");
            UiManager.DebugUi.SetVelocity(totalVelocity);
            UiManager.DebugUi.SetLocalTime(LocalTime.MultiplierAt(transform.position));
        }

        private void OnWallBump() {
            // Kill horizontal movement
            moveVelocity = 0f; // Also done in UpdateWalls()
            jumpVelocity.x = 0f;
            timeHeldMaxXInput = 0f;
            moveTime = 0f;
        }

        private void OnLanding() {
            // Kill vertical movement
            yVelocity = 0f;

            ReplenishAirAttacks();
            ApplyStoredJump();
        }

        private void OnCeilingBump() {
            // Kill jump velocity
            if (jumpVelocity.y > 0f) {
                jumpVelocity.y *= -0.1f;
            }
        }

        #region Horizontal Movement

        [Header("Horizontal Movement")] private readonly float maxWalkSpeed = 8f;

        [Range(1, 30), SerializeField] private float maxRunSpeed = 14f;
        private readonly float timeToHoldMaxXInputToRun = 0.0f; // 0 = always run
        [Range(0, 150), SerializeField] private float accelerationAir = 80f;

        [Range(0, 100), SerializeField] private float accelerationGround = 80f;

        //private float accelerationReverseDirection = 1f;
        [Range(0, 200), SerializeField] private float decceleration = 80f;
        [Range(0f, 5f)] public float slideAccelerationMultiplier = 1.5f;

        private float moveVelocity;
        private float moveTime;
        private float timeHeldMaxXInput;
        private float timeSinceWall = Mathf.Infinity;
        private float wallJumpDirection;
        private bool isRunning;
        private bool isFacingLeft;
        private bool isSliding;

        private void UpdateMoveVelocity(float input) {
            var deltaTime = LocalTime.DeltaTimeAt(this);

            if (Mathf.Abs(input) >= 0.8f) {
                if (controller.isGrounded) {
                    timeHeldMaxXInput += deltaTime;
                }
            } else {
                timeHeldMaxXInput = 0f;
            }

            isRunning = timeHeldMaxXInput >= timeToHoldMaxXInputToRun;
            isSliding = false; // set to true later
            var targetVelocity = input * (isRunning ? maxRunSpeed : maxWalkSpeed);

            if (Mathf.Abs(targetVelocity) == 0f || Mathf.Abs(targetVelocity) < Mathf.Abs(moveVelocity)) {
                // Deccelerate if no input or we are above max speed
                var oldSign = Mathf.Sign(moveVelocity);

                moveVelocity -= Mathf.Sign(moveVelocity) * decceleration * deltaTime;

                // If we've crossed zero, stop
                if (Mathf.Sign(moveVelocity) != oldSign) {
                    moveVelocity = 0f;
                }

                if (Mathf.Abs(moveVelocity) < 0.1f) {
                    moveVelocity = 0f;
                }

                moveTime = 0f;
            } else {
                // Accelerate
                var acceleration = input * (controller.isGrounded ? accelerationGround : accelerationAir) * deltaTime;

                // Reversing direction is faster
                if (Mathf.Sign(moveVelocity) == -Mathf.Sign(acceleration) && controller.isGrounded) {
                    acceleration *= slideAccelerationMultiplier;
                    isSliding = true;
                }

                if (Mathf.Abs(moveVelocity + acceleration) <=
                    Mathf.Abs(targetVelocity)) // If accelerating will not exceed target...
                    // Apply acceleration normally
                {
                    moveVelocity += acceleration;
                } else if (Mathf.Abs(moveVelocity) <
                           Mathf.Abs(targetVelocity)) // If above target velocity already, don't accelerate
                    // We've reached target velocity
                {
                    moveVelocity = targetVelocity;
                }

                if (acceleration < 0f) {
                    isFacingLeft = true;
                } else if (acceleration > 0f) {
                    isFacingLeft = false;
                }

                moveTime += deltaTime;
            }
        }

        private void UpdateWalls() {
            var deltaTime = LocalTime.DeltaTimeAt(this);
            var left = controller.CheckLeft();
            var right = controller.CheckRight();

            if (left || right) {
                timeSinceWall = 0f;
                wallJumpDirection = left ? 1f : -1f;

                // Hitting wall kills x velocity
                if (moveVelocity > 0.1f && right) {
                    moveVelocity = 0.1f;
                } else if (moveVelocity < -0.1f && left) {
                    moveVelocity = -0.1f;
                }

                if (jumpVelocity.y <= 0f) {
                    if (jumpVelocity.x > 0.1f && left) {
                        jumpVelocity.x = 0f;
                    } else if (jumpVelocity.x < -0.1f && right) {
                        jumpVelocity.x = 0f;
                    }
                }

                // Check for stored wall jump
                ApplyStoredJump();
            } else {
                timeSinceWall += deltaTime;
            }
        }

        public bool IsMovingHorizontally() {
            return Mathf.Abs(moveVelocity) > 0.1f;
        }

        public bool IsWallSliding() {
            return timeSinceWall < 0.01f && !controller.isGrounded &&
                   Mathf.Sign(InputManager.PlayerInput.Movement.x) == -wallJumpDirection && jumpVelocity.y == 0f &&
                   moveVelocity != 0f;
        }

        public bool IsWallPushing() {
            return timeSinceWall < 0.01f && controller.isGrounded && Mathf.Abs(moveVelocity) > 0f;
        }

        public bool IsFacingLeft() {
            return isFacingLeft;
        }

        public bool IsSlidng() {
            return isSliding;
        }

        #endregion

        #region Vertical Movement

        [Header("Vertical Movement"), Range(0f, 100f), SerializeField]
        private float walkJumpForce = 20f;

        [Range(0f, 100f), SerializeField] private float runJumpForce = 30f;
        public Vector2 wallJumpForce = new(21f, 20f);
        public Vector2 jumpVelocityDamping = new(0.25f, 0.25f);
        [Range(0f, 1f), SerializeField] private float jumpCoyoteTime = 0.1f;
        [Range(0f, 1f), SerializeField] private float wallJumpCoyoteTime = 0.1f;
        [Range(0f, 100f), SerializeField] private float gravity = 60f;
        [Range(0f, 100f), SerializeField] private float terminalFallVelocity = 25f;
        [Range(0f, 100f), SerializeField] private float wallSlideSpeed = 6f;

        private Vector2 jumpVelocity = Vector2.zero;
        private bool didJumpCancel;
        private float yVelocity; // TODO: rename to fallVelocity?
        private float timeSinceStoredJump = Mathf.Infinity;

        private void ApplyGravity() {
            var deltaTime = LocalTime.DeltaTimeAt(this);

            // Jumping (upwards portion)
            if (jumpVelocity.y > 0f) {
                // Check for jump cancel (jump button released before apex)
                if (!InputManager.PlayerInput.Jump) {
                    didJumpCancel = true;
                }

                // Apply y-axis jump damping (24x if cancelled)
                var yJumpDamping = didJumpCancel ? jumpVelocityDamping.y * 24f : jumpVelocityDamping.y;
                jumpVelocity.y -= jumpVelocity.y * yJumpDamping * deltaTime;

                // If we've passed the apex of the jump, transfer jump y velocity away
                if (jumpVelocity.y + yVelocity < 0f) {
                    // Y: jumpVelocity->0 & yVelocity->0 to apply gravity from zero
                    yVelocity = 0f;
                    jumpVelocity.y = 0f;

                    didJumpCancel = false;
                }
            } else {
                jumpVelocity.y = 0f;
            }

            // Apply x-axis jump damping (24x if grounded)
            var xJumpDamping = controller.isGrounded ? jumpVelocityDamping.x * 24f : jumpVelocityDamping.x;
            jumpVelocity.x -= jumpVelocity.x * xJumpDamping * deltaTime;

            // Gravity
            yVelocity -= gravity * deltaTime;

            // Terminal Y velocity
            var totalVel = yVelocity + jumpVelocity.y;
            var currentTerminalVel = controller.isGrounded ? 0.1f : terminalFallVelocity;
            if (!controller.isGrounded && totalVel <= 0f)

                // Wall slide.
                // Pushing against a wall limits fall speed & faces player away from wall
            {
                if (isPushingWall()) {
                    if (totalVel < 0f) {
                        currentTerminalVel = wallSlideSpeed;
                    }

                    isFacingLeft = controller.CheckRight();

                    // TODO: produce particles
                }
            }

            /*if (!controller.isGrounded && InputManager.PlayerInput.Movement.y < 0) { // Fast fall
                currentTerminalVel *= 4f * Mathf.Abs(InputManager.PlayerInput.Movement.y);
                yVelocity -= 50f * deltaTime;
            }*/
            if (totalVel < -currentTerminalVel) {
                yVelocity = -currentTerminalVel;
            }
        }

        private void OnInputJump(bool isPressed) {
            if (isPressed) {
                if (controller.airTime < jumpCoyoteTime && Jump()) {
                    // We jumped.
                } else if (timeSinceWall < wallJumpCoyoteTime && WallJump()) {
                    // We wall-jumped.
                } else {
                    // Store jump input and apply it when we hit the ground/wall
                    timeSinceStoredJump = 0f;
                }
            }
        }

        // If there's an input stored and its possible to jump, jump and discard the input.
        private bool ApplyStoredJump() {
            if (timeSinceStoredJump < jumpCoyoteTime && Jump()) {
                Debug.Log("Stored jump!");
                timeSinceStoredJump = Mathf.Infinity;
                return true;
            }

            if (timeSinceStoredJump < wallJumpCoyoteTime && WallJump()) {
                Debug.Log("Stored wall jump!");
                timeSinceStoredJump = Mathf.Infinity;
                return true;
            }

            Debug.Log("Stored jump fail");

            return false;
        }

        private bool WallJump() {
            if (controller.isGrounded) {
                return false;
            }

            if (!isPushingWall()) {
                return false;
            }

            if (physicsDisableTime > 0f) {
                return false;
            }

            yVelocity = 0f;

            // wallJumpForce.x is partially given to moveVelocity, the rest is given to jumpVelocity.x
            var moveForce = Mathf.Min(wallJumpForce.x, maxRunSpeed);
            moveVelocity = moveForce * wallJumpDirection;
            jumpVelocity = new Vector2(Mathf.Max(0f, wallJumpForce.x - moveForce) * wallJumpDirection, wallJumpForce.y);

            return true;
        }

        private bool Jump() {
            if (IsJumping()) {
                return false;
            }

            if (!controller.isGrounded) {
                return false;
            }

            if (physicsDisableTime > 0f) {
                return false;
            }

            yVelocity = 0f;
            jumpVelocity = Vector2.up * (isRunning ? runJumpForce : walkJumpForce);

            return true;
        }

        public bool IsJumping() {
            return !controller.isGrounded && jumpVelocity.SqrMagnitude() > 0f;
        }

        public bool IsFalling() {
            return yVelocity + jumpVelocity.y < 0f && !controller.isGrounded;
        }

        public bool IsAirbourne() {
            return !controller.isGrounded;
        }

        private bool isPushingWall() {
            return (controller.CheckLeft() && InputManager.PlayerInput.Movement.x < 0f) ||
                   (controller.CheckRight() && InputManager.PlayerInput.Movement.x > 0f);
        }

        #endregion

        #region Attack

        [Header("Jab")] public float jabForce = 10f;

        public float jabAttackDuration = 0.5f;
        public float jabAttackCooldown = 0.5f;
        public float jabPhysicsDisableTime = 0.2f;
        public float finalJabAttackDuration = 1f;
        public float finalJabAttackCooldown = 1f;
        public float finalJabPhysicsDisableTime = 0.2f;
        public float jabMovementSpeedMultiplier;
        public float jabFallSpeedMultiplier;
        public Vector2 jabJumpSpeedMultiplier = Vector2.zero;
        public int maxAirJabs = -1;
        public AudioSource jabSound;
        [Header("Dash")] public float dashForce = 40f;

        public float dashDuration = 0.4f;
        public float dashCooldown = 0.2f;
        public float dashPhysicsDisableTime = 0.4f;
        public float dashFallSpeedMultiplier;
        public Vector2 dashJumpSpeedMultiplier = Vector2.zero;
        public int maxAirDashes = 1;

        [Header("Uppercut")] public float uppercutForce = 20f;

        public float uppercutDuration = 0.4f;
        public float uppercutCooldown = 0.5f;
        public float uppercutPhysicsDisableTime = 0.1f;
        public float uppercutMovementSpeedMultiplier;
        public float uppercutFallSpeedMultiplier;
        public Vector2 uppercutJumpSpeedMultiplier = Vector2.zero;
        public int maxAirUppercuts = 1;

        [Header("Slam")] public float slamForce = 20f;

        public float slamDuration = Mathf.Infinity;
        public float slamCooldown = 0.5f;
        public float slamPhysicsDisableTime = 0.5f;
        public float slamMovementSpeedMultiplier;
        public float slamFallSpeedMultiplier;
        public Vector2 slamJumpSpeedMultiplier = Vector2.zero;
        public int maxAirSlams = 1;

        public enum AttackType {
            None,
            Jab1,
            Jab2,
            Jab3,
            DashForward,
            DashBackward,
            Uppercut,
            Slam
        }

        private AttackType attackType = AttackType.None;
        private float physicsDisableTime; // Disables movement & gravity if positive
        private float attackDisableTime; // No attacks during cooldown
        private float attackAnimTime;
        private int airJabsRemaining;
        private int airDashesRemaining;
        private int airUppercutsRemaining;
        private int airSlamsRemaining;
        [NonSerialized] public Vector2 AttackKnockback;
        [NonSerialized] public float AttackDamage;

        public AttackType GetAttackType() {
            return attackAnimTime > 0f ? attackType : AttackType.None;
        }

        private void OnInputAttack(bool isPressed) {
            if (isPressed) {
                if (InputManager.PlayerInput.Movement.y > 0) {
                    Uppercut();
                } else if (InputManager.PlayerInput.Movement.y < 0 && !controller.isGrounded) {
                    Slam();
                } else {
                    Jab();
                }
            }
        }

        private void OnInputSpecial(bool isPressed) {
            if (isPressed) {
                // TODO
            }
        }

        private void OnInputDash(bool isPressed) {
            if (isPressed) {
                Dash();
            }
        }

        public bool Jab() {
            var nextType = AttackType.Jab1;
            if (attackDisableTime > 0f) {
                // Combo
                if (attackType == AttackType.Jab1) {
                    nextType = AttackType.Jab2;
                } else if (attackType == AttackType.Jab2) {
                    nextType = AttackType.Jab3;
                } else if (attackType == AttackType.DashForward || attackType == AttackType.DashBackward)

                    // TODO: Suplex
                {
                    nextType = AttackType.Jab1;
                } else {
                    return false;
                }
            }

            if (!controller.isGrounded) {
                if (airJabsRemaining == 0) {
                    return false;
                }

                airJabsRemaining--;
            }

            // Jab is in direction player is facing...
            var direction = isFacingLeft ? -1f : 1f;

            // ...overriden by input.
            if (InputManager.PlayerInput.Movement.x != 0f) {
                direction = Mathf.Sign(InputManager.PlayerInput.Movement.x);
            }

            // Jab!
            attackType = nextType;
            moveVelocity *= jabMovementSpeedMultiplier;
            yVelocity *= jabFallSpeedMultiplier;
            jumpVelocity *= jabJumpSpeedMultiplier;
            jumpVelocity += Vector2.right * direction * jabForce;
            attackAnimTime = attackType == AttackType.Jab3 ? finalJabAttackDuration : jabAttackDuration;
            attackDisableTime = attackType == AttackType.Jab3 ? finalJabAttackCooldown : jabAttackCooldown;
            physicsDisableTime = attackType == AttackType.Jab3 ? finalJabPhysicsDisableTime : jabPhysicsDisableTime;
            AttackKnockback = direction * jabForce * 2f * Vector2.right;
            AttackDamage = 1f;

            jabSound.Play();

            return true;
        }

        public bool Uppercut() {
            if (attackDisableTime > 0f) {
                if (attackType == AttackType.Jab1 || attackType == AttackType.Jab2) {
                    // Combo from non-finishing jab
                } else {
                    return false;
                }
            }

            if (!controller.isGrounded) {
                if (airUppercutsRemaining == 0) {
                    return false;
                }
            }

            if (airUppercutsRemaining > 0) {
                airUppercutsRemaining--; // Uppercut pushes you airbourne
            }

            // Uppercut!
            attackType = AttackType.Uppercut;
            moveVelocity *= uppercutMovementSpeedMultiplier;
            yVelocity *= uppercutFallSpeedMultiplier;
            jumpVelocity *= uppercutJumpSpeedMultiplier;
            jumpVelocity += Vector2.up * uppercutForce;
            attackAnimTime = uppercutDuration;
            attackDisableTime = uppercutCooldown;
            physicsDisableTime = uppercutPhysicsDisableTime;
            AttackKnockback = uppercutForce * Vector2.up;
            AttackDamage = 2f;

            return true;
        }

        public bool Slam() {
            if (attackDisableTime > 0f) {
                if (attackType == AttackType.Jab1 || attackType == AttackType.Jab2) {
                    // Combo from non-finishing jab
                } else {
                    return false;
                }
            }

            if (!controller.isGrounded) {
                if (airSlamsRemaining == 0) {
                    return false;
                }

                airSlamsRemaining--;
            }

            // Slam!
            attackType = AttackType.Slam;
            moveVelocity *= slamMovementSpeedMultiplier;
            yVelocity *= slamFallSpeedMultiplier;
            jumpVelocity *= slamJumpSpeedMultiplier;
            jumpVelocity += Vector2.down * slamForce;
            attackAnimTime = slamDuration;
            attackDisableTime = slamCooldown;
            physicsDisableTime = slamPhysicsDisableTime;
            AttackKnockback = slamForce * Vector2.down;
            AttackDamage = 3f;

            return true;
        }

        public bool Dash() {
            if (attackDisableTime > 0f) {
                if (attackType == AttackType.Jab1) {
                    // TODO: Suplex
                }

                return false;
            }

            if (!controller.isGrounded) {
                if (airDashesRemaining == 0) {
                    return false;
                }

                airDashesRemaining--;
            }

            // Dash is in direction player is facing...
            var direction = isFacingLeft ? -1f : 1f;

            // ...overriden by movement velocity...
            if (moveVelocity != 0f) {
                direction = Mathf.Sign(moveVelocity);
            }

            // ...overriden by input...
            if (InputManager.PlayerInput.Movement.x != 0f) {
                direction = Mathf.Sign(InputManager.PlayerInput.Movement.x);
            }

            // ...overriden by wall if they're against one.
            if (isPushingWall()) {
                direction = wallJumpDirection;
            }

            // If the dash is in the direction of current velocity, it's a dash forward.
            if (Mathf.Sign(moveVelocity) == direction) {
                attackType = AttackType.DashForward;
            } else {
                attackType = AttackType.DashBackward;
            }

            // Dash!
            moveVelocity = direction * dashForce;
            jumpVelocity *= dashJumpSpeedMultiplier;
            yVelocity *= dashFallSpeedMultiplier;
            attackAnimTime = dashDuration;
            attackDisableTime = dashCooldown;
            physicsDisableTime = dashPhysicsDisableTime;

            return true;
        }

        public void ReplenishAirAttacks() {
            airJabsRemaining = maxAirJabs;
            airUppercutsRemaining = maxAirUppercuts;
            airSlamsRemaining = maxAirSlams;
            airDashesRemaining = maxAirDashes;
        }

        private void UpdateAttacks() {
            // If any infinite cooldowns are in progress and we hit the ground, end them.
            if (controller.isGrounded && attackType != AttackType.None) {
                if (physicsDisableTime == Mathf.Infinity) {
                    physicsDisableTime = 0f;
                }

                if (attackDisableTime == Mathf.Infinity) {
                    attackDisableTime = 0f;
                }

                if (attackAnimTime == Mathf.Infinity) {
                    attackAnimTime = 0f;
                }
            }
        }

        #endregion

        #region Damage Effects

        [Header("Normal Attack Effect (Jab1, Jab2)")]
        public float normalAttackEffectDuration = 0.1f;
        public float normalAttackEffectShakeAmplitude = 20f;
        public float normalAttackEffectShakeFrequency = 10f;

        [Header("Strong Attack Effect (Jab3, Uppercut, Slam)")]
        public float strongAttackEffectDuration = 0.15f;
        public float strongAttackEffectShakeAmplitude = 30f;
        public float strongAttackEffectShakeFrequency = 20f;

        private GameObject shakeEffect;
        private GameObject timeEffect;

        public void DidDamageEnemy(Combat.Enemy enemy) {
            // Don't spawn an effect if there's already one running.
            if (shakeEffect != null || timeEffect != null) {
                return;
            }

            switch (attackType) {
                case AttackType.None:
                case AttackType.DashForward:
                case AttackType.DashBackward:
                default:
                    break;
                case AttackType.Jab1:
                case AttackType.Jab2:
                    // Normal attack effect
                    shakeEffect = ShakeEffect.Spawn(0f, normalAttackEffectDuration, normalAttackEffectShakeAmplitude, normalAttackEffectShakeFrequency);
                    timeEffect = TimeEffect.Spawn(normalAttackEffectDuration, 0.12f);
                    break;
                case AttackType.Uppercut:
                case AttackType.Slam:
                case AttackType.Jab3:
                    // Strong attack effect
                    shakeEffect = ShakeEffect.Spawn(0f, strongAttackEffectDuration, strongAttackEffectShakeAmplitude, strongAttackEffectShakeFrequency);
                    timeEffect = TimeEffect.Spawn(strongAttackEffectDuration, 0.12f);
                    break;
            }
        }

        #endregion
    }
}
