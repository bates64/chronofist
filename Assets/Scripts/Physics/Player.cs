using UnityEngine;
using UI;
using General;

namespace Physics {
    [RequireComponent(typeof(Controller2D))]
    [RequireComponent(typeof(Health.Health))]
    public class Player : MonoBehaviour {
        private Controller2D controller;

        private void Awake() {
            controller = GetComponent<Controller2D>();
            controller.collision.OnWallBump += OnWallBump;
            controller.collision.OnLanding += OnLanding;
            controller.collision.OnCeilingBump += OnCeilingBump;

            InputManager.PlayerInput.OnJumpChange += OnInputJump;
        }

        private void Update() {
            UpdateMoveVelocity(InputManager.PlayerInput.Movement.x);
            ApplyGravity();

            Vector2 totalVelocity = new Vector2(moveVelocity, yVelocity);
            controller.Move(totalVelocity);

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

        private float maxWalkSpeed = 0.02f;
        private float maxRunSpeed = 0.06f;
        private float timeToHoldMaxXInputToRun = 0.2f;
        private float accelerationFactorAir = 0.3f;
        private float accelerationFactorGround = 0.4f;
        private float deccelerationFactorAir = 0.3f;
        private float deccelerationFactorGround = 0.8f;

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
            float accelerationFactor = controller.isGrounded ? accelerationFactorGround : accelerationFactorAir;
            float deccelerationFactor = controller.isGrounded ? deccelerationFactorGround : deccelerationFactorAir;

            if (Mathf.Abs(targetVelocity) == 0f) {
                moveVelocity = Mathf.MoveTowards(moveVelocity, 0f, deccelerationFactor * deltaTime);
                moveTime = 0f;
            } else {
                moveVelocity = Mathf.MoveTowards(moveVelocity, targetVelocity, accelerationFactor * deltaTime);
                moveTime += deltaTime;
            }
        }

        #endregion

        #region Vertical Movement

        private float walkJumpForce = 0.09f;
        private float runJumpForce = 0.1f;
        private float wallJumpForce = 0.1f;
        private float jumpCoyoteTime = 0.1f;
        private float gravity = 0.5f;
        private float terminalFallVelocity = 0.2f;

        private float yVelocity = 0f;
        private bool didWallJump = false;

        private void ApplyGravity() {
            float deltaTime = LocalTime.DeltaTimeAt(this);

            float multiplier = 1f;

            // During upwards part of jump, apply less gravity if jump is held
            if (yVelocity > 0f && InputManager.PlayerInput.Jump) {
                multiplier *= 0.5f;
            }

            // Pushing against a wall slows fall speed (sliding)
            if (yVelocity < 0f && InputManager.PlayerInput.Movement.x < -0.1f && controller.CheckLeft()) {
                Debug.Log("slide");
                multiplier *= 0.2f;
            } else if (yVelocity < 0f && InputManager.PlayerInput.Movement.x > 0.1f && controller.CheckRight()) {
                multiplier *= 0.2f;
            }

            yVelocity -= gravity * deltaTime * multiplier;
            if (yVelocity < -terminalFallVelocity) {
                yVelocity = -terminalFallVelocity;
            }
        }

        private void OnInputJump(bool isPressed) {
            if (isPressed) {
                if (controller.isGrounded || controller.airTime < jumpCoyoteTime) {
                    yVelocity = isRunning ? runJumpForce : walkJumpForce;
                } else if (isRunning && (controller.CheckLeft() || controller.CheckRight()) && !didWallJump) { // FIXME: left wall jump is broken
                    yVelocity = wallJumpForce;
                    moveVelocity = controller.CheckLeft() ? maxRunSpeed : -maxRunSpeed;
                    didWallJump = true;
                }
            }
        }

        #endregion
    }
}
