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
            UpdateMoveVelocity(InputManager.PlayerInput.Movement.x);
            ApplyGravity();
            UpdateAttackType();

            Vector2 totalVelocity = new Vector2(moveVelocity, yVelocity);
            controller.Move(totalVelocity * LocalTime.DeltaTimeAt(this));

            UiManager.DebugUi.SetStateName($"cx={InputManager.PlayerInput.Movement.x} checkLeft={controller.CheckLeft()}");
            UiManager.DebugUi.SetVelocity(totalVelocity);
            UiManager.DebugUi.SetLocalTime(LocalTime.MultiplierAt(transform.position));
        }

        private void OnWallBump() {
            // Kill horizontal movement
            moveVelocity = 0f;
            timeHeldMaxXInput = 0f;
            moveTime = 0f;
        }

        private void OnLanding() {
            // Kill vertical movement
            yVelocity = 0f;

            // Allow wall jumps when next airbourne
            didWallJump = false;
        }

        private void OnCeilingBump() {
            // Bounce off ceiling
            if (yVelocity > 0f) {
                yVelocity *= -0.1f;
            }
        }

        #region Horizontal Movement

        private float maxWalkSpeed = 8f;
        private float maxRunSpeed = 14f;
        private float timeToHoldMaxXInputToRun = 0.0f; // 0 = always run
        private float accelerationAir = 80f;
        private float accelerationGround = 80f;
        //private float accelerationReverseDirection = 1f;
        private float decceleration = 80f;

        private float moveVelocity = 0f;
        private float moveTime = 0f;
        private float timeHeldMaxXInput = 0f;
        private bool isRunning = false;

        private void UpdateMoveVelocity(float input) {
            float deltaTime = LocalTime.DeltaTimeAt(this);

            if (Mathf.Abs(input) >= 0.8f) {
                if (controller.isGrounded)
                    timeHeldMaxXInput += deltaTime;
            } else {
                timeHeldMaxXInput = 0f;
            }

            isRunning = timeHeldMaxXInput >= timeToHoldMaxXInputToRun;
            float targetVelocity = input * (isRunning ? maxRunSpeed : maxWalkSpeed);

            if (Mathf.Abs(targetVelocity) == 0f) {
                var oldSign = Mathf.Sign(moveVelocity);

                moveVelocity -= Mathf.Sign(moveVelocity) * decceleration * deltaTime;

                if (Mathf.Sign(moveVelocity) != oldSign)
                    moveVelocity = 0f;
                if (Mathf.Abs(moveVelocity) < 0.1f)
                    moveVelocity = 0f;

                moveTime = 0f;
            } else {
                float acceleration = controller.isGrounded ? accelerationGround : accelerationAir;
                //if (Mathf.Sign(targetVelocity) != Mathf.Sign(moveVelocity))
                //    acceleration = accelerationReverseDirection;

                var oldSign = Mathf.Sign(moveVelocity);

                moveVelocity += input * acceleration * deltaTime;
                moveTime += deltaTime;

                if (Mathf.Sign(moveVelocity) != oldSign) {
                    // TOOD: turn around really fast
                } else if (Mathf.Abs(moveVelocity) > Mathf.Abs(targetVelocity)) {
                    moveVelocity = targetVelocity;
                }
            }
        }

        #endregion

        #region Vertical Movement

        [Range(0f,100f)][SerializeField] private float walkJumpForce = 16f;
        [Range(0f,100f)][SerializeField] private float runJumpForce = 20f;
        [Range(0f,100f)][SerializeField] private float wallJumpForce = 16f;
        [Range(0f,100f)][SerializeField] private float jumpCoyoteTime = 0.1f;
        [Range(0f,100f)][SerializeField] private float gravity = 40f;
        [Range(0f,100f)][SerializeField] private float terminalFallVelocity = 20f;

        private float yVelocity = 0f;
        private bool didWallJump = false;

        private void ApplyGravity() {
            float deltaTime = LocalTime.DeltaTimeAt(this);
            float currentTerminalVel = controller.isGrounded ? 0.1f : terminalFallVelocity;
            float multiplier = 1f;

            // During upwards part of jump, apply less gravity if jump is held
            if (yVelocity > 0f && InputManager.PlayerInput.Jump) {
                multiplier *= 0.5f;
            }

            // Pushing against a wall slows fall speed (sliding)
            if (yVelocity < 0f && InputManager.PlayerInput.Movement.x < -0.1f && (controller.CheckLeft()) && !controller.isGrounded) {
                multiplier *= 0.2f;
            } else if (yVelocity < 0f && InputManager.PlayerInput.Movement.x > 0.1f && controller.CheckRight() && !controller.isGrounded) {
                multiplier *= 0.2f;
            }
            yVelocity -= gravity * deltaTime * multiplier;
            if (yVelocity < -currentTerminalVel) {
                yVelocity = -currentTerminalVel;
            }
        }

        private void OnInputJump(bool isPressed) {
            if (isPressed) {
                if (controller.isGrounded || controller.airTime < jumpCoyoteTime) {
                    yVelocity = isRunning ? runJumpForce : walkJumpForce;
                } else if ((controller.CheckLeft() || controller.CheckRight()) && !didWallJump) {
                    yVelocity = wallJumpForce;
                    moveVelocity = controller.CheckLeft() ? maxRunSpeed : -maxRunSpeed;
                    didWallJump = true;
                }
            }
            else
            {
                if (yVelocity > 0)
                {
                    yVelocity = 0;
                }
            }
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
