using General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class PlayerInputs
    {
        private InputActionsPlayer _playerInput;
        private Vector2 _movement;
        private bool _jump;
        private bool _attack;
        public event Util.DVector2 OnMovementChange;
        public event Util.DBool OnJumpChange;
        public event Util.DBool OnAttackChange;
        
        #region Properties

        public Vector2 Movement => _movement;
        public bool Jump => _jump;
        public bool Attack => _attack;

        #endregion

        #region Setup Functions

        public void Init()
        {
            _playerInput = new InputActionsPlayer();
            _playerInput.Enable();
            _playerInput.Main.Move.performed += RelayMovement;
            _playerInput.Main.Move.canceled += RelayMovement;
            _playerInput.Main.Jump.performed += RelayJump;
            _playerInput.Main.Jump.canceled += RelayJump;
            _playerInput.Main.Attack.performed += RelayAttack;
            _playerInput.Main.Attack.canceled += RelayAttack;
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
        
        #endregion
        
        public void Disable()
        {
            _playerInput.Disable();
        }
    }
}