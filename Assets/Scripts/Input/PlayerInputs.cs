using General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input {
    public class PlayerInputs : InputSystemConsumer {
        private Vector2 _movement = Vector2.zero;
        private bool _jump = false;
        private bool _attack = false;
        public event Util.DVector2 OnMovementChange;
        public event Util.DBool OnJumpChange;
        public event Util.DBool OnAttackChange;
        public event Util.DVoid OnPause;

        #region Properties

        public Vector2 Movement => _movement;
        public bool Jump => _jump;
        public bool Attack => _attack;

        #endregion

        #region Setup Functions

        public PlayerInputs(): base() {
            _actions.Main.Move.performed += RelayMovement;
            _actions.Main.Move.canceled += RelayMovement;
            _actions.Main.Jump.performed += RelayJump;
            _actions.Main.Jump.canceled += RelayJump;
            _actions.Main.Attack.performed += RelayAttack;
            _actions.Main.Attack.canceled += RelayAttack;
            _actions.Main.Pause.performed += RelayPause;
            _actions.Main.Pause.canceled += RelayPause;
        }
        
        #endregion

        #region Input Functions

        private void RelayMovement(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<Vector2>();
            OnMovementChange?.Invoke(_movement);
        }

        private void RelayJump(InputAction.CallbackContext context)
        {
            _jump = context.ReadValueAsButton();
            OnJumpChange?.Invoke(_jump);
        }

        private void RelayAttack(InputAction.CallbackContext context)
        {
            _attack = context.ReadValueAsButton();
            OnAttackChange?.Invoke(_attack);
        }

        private void RelayPause(InputAction.CallbackContext context) {
            if (context.ReadValueAsButton()) {
                OnPause?.Invoke();
            }
        }

        #endregion
    }
}