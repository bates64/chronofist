using UnityEngine;
using Health;

namespace Physics {
    [RequireComponent(typeof(Controller2D))]
    [RequireComponent(typeof(Health.Health))]
    public class Player : MonoBehaviour {
        private Controller2D controller;

        public Vector2 velocity;

        private void Awake() {
            controller = GetComponent<Controller2D>();
        }

        private void Update() {
            ApplyVelocityFromInput(InputManager.PlayerInput.Movement);
            controller.Move(velocity);
        }

        private void ApplyVelocityFromInput(Vector2 input) {
            // TODO: actual platformer physics
            velocity.x = input.x * 0.1f;
            velocity.y = input.y * 0.1f;
        }
    }
}
