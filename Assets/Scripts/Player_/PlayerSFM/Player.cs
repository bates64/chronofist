using System;
using Physics;
using Player_.PlayerSFM.States;
using Player_.PlayerSFM.States.BaseClasses;
using UI;
using UnityEngine;

namespace Player_.PlayerSFM
{
    [RequireComponent(typeof(Controller2D))]
    public class Player : MonoBehaviour
    {
        [Header("Movement Properties")] 
        [SerializeField] private float jumpHeight;
        [SerializeField] private float timeToJumpApex;
        [SerializeField] private float movementSpeed;
        private Gravity _gravity;
        private Vector2 _velocity;
        private float _jumpVelocity;
        private Controller2D _controller2D;
        private PlayerState _currentState;
        public readonly PlayerStates States = new PlayerStates();

        #region Properties

        public float MovementSpeed => movementSpeed;
        public float JumpVelocity => _jumpVelocity;

        public Vector2 Velocity
        {
            private set
            {
                _velocity = value;
                UiManager.DebugUi.SetVelocity(_velocity);
            }
            get
            {
                return _velocity;
            }
        }
        public Gravity Gravity => _gravity;
        public Controller2D Controller2D => _controller2D;
        
        #endregion

        #region Unity Event and Setup Functions

        private void Awake()
        {
            _controller2D = GetComponent<Controller2D>();
            SetupJump();
            States.Init();
            SetState(States.FallingState);
        }
        
        private void Update()
        {
            Velocity = Vector2.zero;
            _currentState?.Update();

            UiManager.DebugUi.SetLocalTime(LocalTime.multiplierAt(transform.position));
        }

        private void SetupJump()
        {
            float gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            _jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            _gravity = new Gravity(gravity);
        }
        
        #endregion
        
        public void SetState(PlayerState nextState)
        {
            _currentState?.ExitState();
            _currentState = nextState;
            DebugPostState();
            _currentState?.EnterState(this);
        }

        public void Move(Vector2 velocity)
        {
            _controller2D.Move(velocity);
        }
        
        public void AddVelocity(Vector2 velocity)
        {
            Velocity += velocity;
        }

        private void DebugPostState()
        {
            UiManager.DebugUi.SetStateName(_currentState.ToString());
            //print(_currentState.ToString());
        }

    }
}