using UnityEngine;
using UnityEngine.InputSystem;

namespace Input {
    public class InterfaceInputs : InputSystemConsumer {
        public bool moveUp { get; private set; } = false;
        public bool moveDown { get; private set; } = false;
        public bool moveLeft { get; private set; } = false;
        public bool moveRight { get; private set; } = false;

        public bool back { get; private set; } = false;
        public bool interact { get; private set; } = false;

        public Vector2Int movement => getMovement();

        public InterfaceInputs(): base() {
            _actions.Interface.MoveUp.performed += RelayMoveUp;
            _actions.Interface.MoveUp.canceled += RelayMoveUp;
            _actions.Interface.MoveDown.performed += RelayMoveUp;
            _actions.Interface.MoveDown.canceled += RelayMoveUp;
            _actions.Interface.MoveLeft.performed += RelayMoveUp;
            _actions.Interface.MoveLeft.canceled += RelayMoveUp;
            _actions.Interface.MoveRight.performed += RelayMoveUp;
            _actions.Interface.MoveRight.canceled += RelayMoveUp;
            _actions.Interface.Back.performed += RelayBack;
            _actions.Interface.Back.canceled += RelayBack;
            _actions.Interface.Interact.performed += RelayInteract;
            _actions.Interface.Interact.canceled += RelayInteract;
        }

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
            Vector2Int movement = Vector2Int.zero;
            if (moveUp) movement.y += 1;
            if (moveDown) movement.y -= 1;
            if (moveLeft) movement.x -= 1;
            if (moveRight) movement.x += 1;
            return movement;
        }
    }
}
