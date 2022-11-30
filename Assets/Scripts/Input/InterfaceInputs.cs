using UnityEngine;
using UnityEngine.InputSystem;

namespace Input {
    public class InterfaceInputs : InputSystemConsumer {
        public InterfaceInputs() {
            _actions.Interface.MoveUp.performed += RelayMoveUp;
            _actions.Interface.MoveUp.canceled += RelayMoveUp;
            _actions.Interface.MoveDown.performed += RelayMoveDown;
            _actions.Interface.MoveDown.canceled += RelayMoveDown;
            _actions.Interface.MoveLeft.performed += RelayMoveLeft;
            _actions.Interface.MoveLeft.canceled += RelayMoveLeft;
            _actions.Interface.MoveRight.performed += RelayMoveRight;
            _actions.Interface.MoveRight.canceled += RelayMoveRight;
            _actions.Interface.Back.performed += RelayBack;
            _actions.Interface.Back.canceled += RelayBack;
            _actions.Interface.Interact.performed += RelayInteract;
            _actions.Interface.Interact.canceled += RelayInteract;
        }

        public bool moveUp { get; private set; }
        public bool moveDown { get; private set; }
        public bool moveLeft { get; private set; }
        public bool moveRight { get; private set; }

        public bool back { get; private set; }
        public bool interact { get; private set; }

        public Vector2Int movement => getMovement();

        private void RelayMoveUp(InputAction.CallbackContext context) {
            moveUp = context.ReadValueAsButton();
        }

        private void RelayMoveDown(InputAction.CallbackContext context) {
            moveDown = context.ReadValueAsButton();
        }

        private void RelayMoveLeft(InputAction.CallbackContext context) {
            moveLeft = context.ReadValueAsButton();
        }

        private void RelayMoveRight(InputAction.CallbackContext context) {
            moveRight = context.ReadValueAsButton();
        }

        private void RelayBack(InputAction.CallbackContext context) {
            back = context.ReadValueAsButton();
        }

        private void RelayInteract(InputAction.CallbackContext context) {
            interact = context.ReadValueAsButton();
        }

        private Vector2Int getMovement() {
            var movement = Vector2Int.zero;
            if (moveUp) {
                movement.y += 1;
            }

            if (moveDown) {
                movement.y -= 1;
            }

            if (moveLeft) {
                movement.x -= 1;
            }

            if (moveRight) {
                movement.x += 1;
            }

            return movement;
        }
    }
}
