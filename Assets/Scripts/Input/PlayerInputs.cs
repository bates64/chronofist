using General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input {
    public class PlayerInputs : InputSystemConsumer {
        #region Setup Functions

        public PlayerInputs() {
            _actions.Main.Move.performed += RelayMovement;
            _actions.Main.Move.canceled += RelayMovement;
            _actions.Main.Jump.performed += RelayJump;
            _actions.Main.Jump.canceled += RelayJump;
            _actions.Main.Attack.performed += RelayAttack;
            _actions.Main.Attack.canceled += RelayAttack;
            _actions.Main.Special.performed += RelaySpecial;
            _actions.Main.Special.canceled += RelaySpecial;
            _actions.Main.Dash.performed += RelayDash;
            _actions.Main.Dash.canceled += RelayDash;
            _actions.Main.Pause.performed += RelayPause;
            _actions.Main.Pause.canceled += RelayPause;
        }

        #endregion

        public event Util.DVector2 OnMovementChange;
        public event Util.DBool OnJumpChange;
        public event Util.DBool OnAttackChange;
        public event Util.DBool OnSpecialChange;
        public event Util.DBool OnDashChange;
        public event Util.DVoid OnPause;

        #region Properties

        public Vector2 Movement { get; private set; } = Vector2.zero;

        public bool Jump { get; private set; }

        public bool Attack { get; private set; }

        public bool Special { get; private set; }

        public bool Dash { get; private set; }

        #endregion

        #region Input Functions

        private void RelayMovement(InputAction.CallbackContext context) {
            Movement = context.ReadValue<Vector2>();
            OnMovementChange?.Invoke(Movement);
        }

        private void RelayJump(InputAction.CallbackContext context) {
            Jump = context.ReadValueAsButton();
            OnJumpChange?.Invoke(Jump);
        }

        private void RelayAttack(InputAction.CallbackContext context) {
            Attack = context.ReadValueAsButton();
            OnAttackChange?.Invoke(Attack);
        }

        private void RelaySpecial(InputAction.CallbackContext context) {
            Special = context.ReadValueAsButton();
            OnSpecialChange?.Invoke(Special);
        }

        private void RelayDash(InputAction.CallbackContext context) {
            Dash = context.ReadValueAsButton();
            OnDashChange?.Invoke(Dash);
        }

        private void RelayPause(InputAction.CallbackContext context) {
            if (context.ReadValueAsButton()) OnPause?.Invoke();
        }

        #endregion
    }
}
