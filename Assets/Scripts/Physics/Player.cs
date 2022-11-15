using UnityEngine;
using UI;
using General;

namespace Physics {
    [RequireComponent(typeof(Controller2D))]
    [RequireComponent(typeof(Health.Health))]
    public class Player : MonoBehaviour {
        public GameObject jabAttackPrefab;

        private Controller2D controller;

        private void Awake() {
            controller = GetComponent<Controller2D>();
            controller.collision.OnWallBump += OnWallBump;
            controller.collision.OnLanding += OnLanding;
            controller.collision.OnCeilingBump += OnCeilingBump;

            InputManager.PlayerInput.OnJumpChange += OnInputJump;
            InputManager.PlayerInput.OnAttackChange += OnInputAttack;
        }

        private void Update() {
            float deltaTime = LocalTime.DeltaTimeAt(this);

            timeSinceStoredJump += deltaTime;

            UpdateMoveVelocity(InputManager.PlayerInput.Movement.x);
            ApplyGravity();
            UpdateWalls();
            UpdateAttackType();

            Vector2 totalVelocity = new Vector2(moveVelocity, yVelocity) + jumpVelocity;
            controller.Move(totalVelocity * LocalTime.DeltaTimeAt(this));

            UiManager.DebugUi.SetStateName($"jumpVelocity={jumpVelocity}");
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

            ApplyStoredJump();
        }

        private void OnCeilingBump() {
            // Kill jump velocity
            if (jumpVelocity.y > 0f) {
                jumpVelocity.y *= -0.1f;
            }
        }

        #region Horizontal Movement

        [Header("Horizontal Movement")]

        private float maxWalkSpeed = 8f;
        [Range(1,30)][SerializeField] private float maxRunSpeed = 14f;
        private float timeToHoldMaxXInputToRun = 0.0f; // 0 = always run
        [Range(0,150)][SerializeField] private float accelerationAir = 80f;
        [Range(0,100)][SerializeField] private float accelerationGround = 80f;
        //private float accelerationReverseDirection = 1f;
        [Range(0,200)][SerializeField] private float decceleration = 80f;

        private float moveVelocity = 0f;
        private float moveTime = 0f;
        private float timeHeldMaxXInput = 0f;
        private float timeSinceWall = Mathf.Infinity;
        private float wallJumpDirection = 0f;
        private bool isRunning = false;
        private bool isWallJumping = false; // Set to false at apex
        private bool isFacingLeft = false;

        private void UpdateMoveVelocity(float input) {
            float deltaTime = LocalTime.DeltaTimeAt(this);

            // No horizontal movement control until wall jump apex
            if (isWallJumping) {
                return;
            }

            if (Mathf.Abs(input) >= 0.8f) {
                if (controller.isGrounded)
                    timeHeldMaxXInput += deltaTime;
            } else {
                timeHeldMaxXInput = 0f;
            }

            isRunning = timeHeldMaxXInput >= timeToHoldMaxXInputToRun;
            float targetVelocity = input * (isRunning ? maxRunSpeed : maxWalkSpeed);

            if (Mathf.Abs(targetVelocity) == 0f || Mathf.Abs(targetVelocity) < Mathf.Abs(moveVelocity)) { // Deccelerate if no input or we are above max speed
                var oldSign = Mathf.Sign(moveVelocity);

                moveVelocity -= Mathf.Sign(moveVelocity) * decceleration * deltaTime;

                // If we've crossed zero, stop
                if (Mathf.Sign(moveVelocity) != oldSign)
                    moveVelocity = 0f;
                if (Mathf.Abs(moveVelocity) < 0.1f)
                    moveVelocity = 0f;

                moveTime = 0f;
            } else {
                // Accelerate
                float acceleration = input * (controller.isGrounded ? accelerationGround : accelerationAir) * deltaTime;

                // Reversing direction is faster
                if (acceleration < 0f && isRunning && controller.isGrounded)
                    acceleration *= 2f; // TODO: slide anim

                if (Mathf.Abs(moveVelocity + acceleration) <= Mathf.Abs(targetVelocity)) { // If accelerating will not exceed target...
                    // Apply acceleration normally
                    moveVelocity += acceleration;
                } else if (Mathf.Abs(moveVelocity) < Mathf.Abs(targetVelocity)) { // If above target velocity already, don't accelerate
                    // We've reached target velocity
                    moveVelocity = targetVelocity;
                }

                if (acceleration < 0f)
                    isFacingLeft = true;
                else if (acceleration > 0f)
                    isFacingLeft = false;

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

                // Check for stored wall jump
                ApplyStoredJump();
            } else {
                timeSinceWall += deltaTime;
            }
        }

        public bool IsMovingHorizontally() {
            return Mathf.Abs(moveVelocity) > 0.1f;
        }

        public bool IsWallClinging() {
            return timeSinceWall < 0.1f;
        }

        public bool IsFacingLeft() {
            return isFacingLeft;
        }

        #endregion

        #region Vertical Movement

        [Header("Vertical Movement")]
        [Range(0f,100f)][SerializeField] private float walkJumpForce = 20f;
        [Range(0f,100f)][SerializeField] private float runJumpForce = 30f;
        public Vector2 wallJumpForce = new Vector2(21f, 20f);
        public Vector2 jumpVelocityDamping = new Vector2(0.25f, 0.25f);
        [Range(0f,1f)][SerializeField] private float jumpCoyoteTime = 0.1f;
        [Range(0f,1f)][SerializeField] private float wallJumpCoyoteTime = 0.1f;
        [Range(0f,100f)][SerializeField] private float gravity = 60f;
        [Range(0f,100f)][SerializeField] private float terminalFallVelocity = 25f;
        [Range(0f,100f)][SerializeField] private float wallSlideSpeed = 6f;

        private Vector2 jumpVelocity = Vector2.zero;
        bool didJumpCancel = false;
        private float yVelocity = 0f; // TODO: rename to fallVelocity?
        private float timeSinceStoredJump = Mathf.Infinity;

        private void ApplyGravity() {
            float deltaTime = LocalTime.DeltaTimeAt(this);

            // Jumping
            if (jumpVelocity.SqrMagnitude() > 1f) {
                // Check for jump cancel (jump button released before apex)
                if (!InputManager.PlayerInput.Jump) {
                    didJumpCancel = true;
                }

                // Apply jump damping (30x if cancelled)
                var damping = didJumpCancel ? jumpVelocityDamping * 30f : jumpVelocityDamping;
                jumpVelocity -= jumpVelocity * damping * deltaTime;

                // If we've passed the apex of the jump or the user wall jump cancelled, transfer jump velocity away
                bool reachedApex = (jumpVelocity.y + yVelocity) < 0f;
                if (reachedApex || checkWallJumpCancel()) {
                    // X: jumpVelocity->moveVelocity
                    moveVelocity = jumpVelocity.x;

                    // Y: jumpVelocity->0 & yVelocity->0 to apply gravity from zero
                    yVelocity = 0f;

                    jumpVelocity = Vector2.zero;

                    isWallJumping = false;
                }
            } else {
                // We're not on the upwards part of the jump
                jumpVelocity = Vector2.zero;
                didJumpCancel = false;
            }

            // Gravity
            yVelocity -= gravity * deltaTime;

            // TerminalY velocity
            float totalVel = yVelocity + jumpVelocity.y;
            float currentTerminalVel = controller.isGrounded ? 0.1f : terminalFallVelocity;
            if (!controller.isGrounded) {
                // Wall slide.
                // Pushing against a wall limits fall speed & faces player away from wall
                bool isPushing = (controller.CheckLeft() && moveVelocity < 0f) || (controller.CheckRight() && moveVelocity > 0f);
                if (isPushing) {
                    if (totalVel < 0f)
                        currentTerminalVel = wallSlideSpeed;
                    isFacingLeft = controller.CheckRight();

                    // TODO: produce particles
                }
            }
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
            if (controller.isGrounded) return false;
            if (!controller.CheckLeft() && !controller.CheckRight()) return false;

            yVelocity = 0f;
            jumpVelocity = wallJumpForce;
            jumpVelocity.x *= wallJumpDirection;

            isWallJumping = true; // No control for a bit

            return true;
        }

        private bool Jump() {
            if (IsJumping()) return false;
            if (!controller.isGrounded) return false;

            yVelocity = 0f;
            jumpVelocity = Vector2.up * (isRunning ? runJumpForce : walkJumpForce);

            return true;
        }

        public bool IsJumping() {
            return !controller.isGrounded && jumpVelocity.SqrMagnitude() > 0f;
        }

        public bool IsFalling() {
            return (yVelocity + jumpVelocity.y) < 0f && !controller.isGrounded;
        }

        // Wall jump cancels are when you release the opposite direction of the direction you are wall-jumping off of.
        // e.g. player wall-jumps off the left wall, then pushes the stick in the left direction to cancel it.
        private bool _wallJumpCancel_previousIsInputDown = false;
        private bool checkWallJumpCancel() {
            if (!isWallJumping) return false;

            bool isInputDown = false;
            if (wallJumpDirection <= 0)
                isInputDown = InputManager.PlayerInput.Movement.x <= 0;
            else
                isInputDown = InputManager.PlayerInput.Movement.x >= 0;

            bool isRisingEdge = !isInputDown && _wallJumpCancel_previousIsInputDown;
            _wallJumpCancel_previousIsInputDown = isInputDown;

            return isRisingEdge;
        }

        #endregion

        #region Attack

        enum AttackDirection {
            Up,
            Down,
            Left,
            Right,
        }

        private AttackDirection attackDirection = AttackDirection.Left;

        private void UpdateAttackType() {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            Vector3 direction = (mousePosition - transform.position).normalized;

            // Direction to degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Normalize angle to 0-360
            if (angle < 0f) {
                angle += 360f;
            }

            // Convert to attack type
            if (angle >= 45f && angle < 135f) {
                attackDirection = AttackDirection.Up;
            } else if (angle >= 135f && angle < 225f) {
                attackDirection = AttackDirection.Left;
            } else if (angle >= 225f && angle < 315f) {
                attackDirection = AttackDirection.Down;
            } else {
                attackDirection = AttackDirection.Right;
            }
        }

        private void OnInputAttack(bool isPressed) {
            if (isPressed) {
                switch (attackDirection) {
                    case AttackDirection.Up:
                        // TODO
                        break;
                    case AttackDirection.Down:
                        // TODO
                        break;
                    case AttackDirection.Left:
                    case AttackDirection.Right:
                        SpawnJabAttack(attackDirection == AttackDirection.Left);
                        break;
                }
            }
        }

        private void SpawnJabAttack(bool isLeft) {
            if (jabAttackPrefab == null) {
                Debug.LogError("Jab attack prefab not set");
                return;
            }

            var jab = Instantiate(jabAttackPrefab, transform.position + new Vector3(isLeft ? -1f : 1f, 0f, 0f), Quaternion.identity);
            jab.transform.localScale = new Vector3(isLeft ? -1f : 1f, 1f, 1f);
        }

        #endregion
    }
}
