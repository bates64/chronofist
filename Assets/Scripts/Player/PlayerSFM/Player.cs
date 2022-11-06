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
        private Vector2 _pushVelocity;
        private Vector2 _movementVelocity;
        public readonly PlayerStates States = new PlayerStates();

        #region Properties

        public PlayerProperties Properties => _properties;
        public float MovementSpeed => _properties.MovementSpeed;
        public float AccelerationFactor => _properties.AccelerationFactor;
        public float DecelerationFactor => _properties.DecelerationFactor;
        public float JumpVelocity => _properties.JumpVelocity;

        /// <summary>
        /// Push velocity, used for things that aren't the player's movement.
        /// Set to zero after being applied.
        /// </summary>
        public Vector2 PushVelocity {
            set {
                
                _pushVelocity = value;
                UiManager.DebugUi.SetVelocity(_pushVelocity + _movementVelocity);
            }
            get {
                return _pushVelocity;
            }
        }

        /// <summary>
        /// Movement velocity, stored between frames.
        /// </summary>
        public Vector2 MovementVelocity {
            set {
                _movementVelocity = value;
                UiManager.DebugUi.SetVelocity(_pushVelocity + _movementVelocity);
            }
            get {
                return _movementVelocity;
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

        private void Update() {
            _currentState?.Update();
            SearchItems();
            UiManager.DebugUi.SetLocalTime(LocalTime.multiplierAt(transform.position));

            if (float.IsNaN(transform.position.x) || float.IsNaN(transform.position.y)) {
                Debug.LogError("Player position is NaN");
            }
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

        public void Push(Vector2 velocity)
        {
            PushVelocity += velocity;
        }

        private void DebugPostState()
        {
            UiManager.DebugUi.SetStateName(_currentState.ToString());
        }

    }
}