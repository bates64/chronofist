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

            UiManager.DebugUi.SetStateName(controller.isGrounded ? $"ground ({controller.groundTime})" : $"air ({controller.airTime})");
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
        private float timeToHoldMaxXInputToRun = 1.0f;
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
        private float runJumpForce = 0.12f;
        private float jumpCoyoteTime = 0.1f;
        private float gravity = 0.5f;
        private float terminalFallVelocity = 0.2f;

        private float yVelocity = 0f;

        private void ApplyGravity() {
            float deltaTime = LocalTime.DeltaTimeAt(this);

            float multiplier = 1f;
            if (yVelocity > 0f && InputManager.PlayerInput.Jump) {
                multiplier = 0.5f;
            }

            yVelocity -= gravity * deltaTime * multiplier;
            if (yVelocity < -terminalFallVelocity) {
                yVelocity = -terminalFallVelocity;
            }
        }

        private void OnInputJump(bool isPressed) {
            if (isPressed && (controller.isGrounded || controller.airTime < jumpCoyoteTime)) {
                yVelocity = isRunning ? runJumpForce : walkJumpForce;
            }
        }

        #endregion
    }
}
