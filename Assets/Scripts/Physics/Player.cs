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
        }

        private void Update() {
            UpdateMoveVelocity(InputManager.PlayerInput.Movement.x);
            ApplyGravity();
            UpdateJump(InputManager.PlayerInput.Jump);

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
            jumpVelocity = 0f;
            timeHeldJumpInput = 0f;
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

        private float jumpVelocity = 2f;
        private float jumpTime = 0.2f;
        private float gravity = 0.01f;
        private float terminalFallVelocity = 1f;

        private float yVelocity = 0f;
        private float timeHeldJumpInput = 0f;

        private void ApplyGravity() {
            float deltaTime = LocalTime.DeltaTimeAt(this);

            if (!controller.isGrounded) {
                yVelocity -= gravity * deltaTime;
                if (yVelocity < -terminalFallVelocity) {
                    yVelocity = -terminalFallVelocity;
                }
            }
        }

        private void UpdateJump(bool input) {
            float deltaTime = LocalTime.DeltaTimeAt(this);

            if (input) {
                timeHeldJumpInput += deltaTime;
            } else {
                timeHeldJumpInput = 0f;
            }

            if (input && controller.isGrounded) {
                yVelocity = jumpVelocity;
                // FIXME
            }
        }

        #endregion
    }
}
