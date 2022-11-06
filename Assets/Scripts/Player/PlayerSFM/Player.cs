using System;
using System.Collections.Generic;
using PassiveItems;
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
        [SerializeField] private PlayerProperties _properties = new PlayerProperties();
        private Controller2D _controller2D;
        private PlayerState _currentState;
        private Vector2 _velocity;
        public readonly PlayerStates States = new PlayerStates();

        #region Properties

        public PlayerProperties Properties => _properties;
        public float MovementSpeed => _properties.MovementSpeed;
        public float JumpVelocity => _properties.JumpVelocity;

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
        public Gravity Gravity => _properties.Gravity;
        public Controller2D Controller2D => _controller2D;
        public PlayerState CurrentState => _currentState;
        #endregion

        #region Unity Event and Setup Functions

        private void Awake()
        {
            _controller2D = GetComponent<Controller2D>();
            _properties.Init(gameObject);
            States.Init();
            SetState(States.FallingState);
        }
        
        private void Update()
        {
            Velocity = Vector2.zero;
            _currentState?.Update();
            SearchItems();
            UiManager.DebugUi.SetLocalTime(LocalTime.multiplierAt(transform.position));
        }

        private void SearchItems()
        {
            List<Pickup> foundItems = new List<Pickup>();
            foreach (var pickup in Pickup.ActivePickups)
            {
                Vector2 dif = transform.position - pickup.transform.position;
                if(Mathf.Abs(dif.magnitude) <= pickup.Radius) foundItems.Add(pickup);
            }
            foreach (var pickup in foundItems)
            {
                pickup.PickUp(this);
            }
            foundItems.Clear();
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
        }

    }
}