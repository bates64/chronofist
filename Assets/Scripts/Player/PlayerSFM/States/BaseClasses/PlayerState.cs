using System.Collections.Generic;
using General;
using UnityEngine;

namespace Player_.PlayerSFM.States.BaseClasses
{
    public abstract class PlayerState : FsmState<Player>
    {
        protected Player Player => Machine;
        protected readonly List<Util.DBool> Subscriptions = new List<Util.DBool>();
        public abstract int StateId
        {
            get;
        }

        public virtual void Init()
        {
            RegisterSubscriptions();
        }
        
        public override void EnterState(Player machine)
        {
            base.EnterState(machine);
            SubscribeInput();
        }

        public override void ExitState()
        {
            UnsubscribeInput();
            base.ExitState();
        }

        protected void SubscribeInput()
        {
            foreach (var sub in Subscriptions)
            {
                sub.Invoke(true);
            }   
        }

        protected void UnsubscribeInput()
        {
            foreach (var sub in Subscriptions)
            {
                sub.Invoke(false);
            }   
        }

        protected abstract void RegisterSubscriptions();
        
        #region Common Update Functions

        protected void ApplyGravity()
        {
            Player.Gravity.AddForce(Physics.LocalTime.deltaTimeAt(Player.transform.position));
            float realVel = Player.Gravity.AccumulatedVelocity + Player.PushVelocity.y;
            float multiplier = realVel <= 0 ? 2 : 1;
            Player.Push(new Vector2(0, realVel * multiplier));
        }

        protected void ApplyAscendingVelocity(float ascendingVelocity)
        {
            Player.Push(new Vector2(0,ascendingVelocity));
        }

        protected void ApplyMovementVelocity() {
            var input = InputManager.PlayerInput.Movement.x;
            var isAccelerating = Mathf.Abs(input) > 0.01f;
            var dt = Physics.LocalTime.deltaTimeAt(Player.transform.position);
            var vel = Player.MovementVelocity;

            if (isAccelerating) {
                vel.x += dt * Player.MovementSpeed * input * Player.AccelerationFactor;

                if (Mathf.Abs(vel.x) > Player.MovementSpeed) {
                    vel.x = Mathf.Sign(vel.x) * Player.MovementSpeed;
                }
            } else {
                vel *= dt * Player.DecelerationFactor;
            }
        }

        protected void PerformMovement()
        {
            Player.Move((Player.PushVelocity + Player.MovementVelocity) * Physics.LocalTime.deltaTimeAt(Player.transform.position));
            Player.PushVelocity = Vector2.zero;
        }

        protected void CommonUpdate()
        {
            ApplyGravity();
            ApplyMovementVelocity();
            PerformMovement();
        }
        
        #endregion

        #region Common Subscription Functions
        
        protected void SubscribeLanding(bool subscribe)
        {
            if(subscribe) Player.Controller2D.Collision.OnLanding += OnLanding;
            else Player.Controller2D.Collision.OnLanding -= OnLanding;
        }

        protected void SubscribeOnTakeOff(bool subscribe)
        {
            if(subscribe) Player.Controller2D.Collision.OnTakeoff += OnTakeOff;
            else Player.Controller2D.Collision.OnTakeoff -= OnTakeOff;
        }

        protected void SubscribeOnCeilingBump(bool subscribe)
        {
            if(subscribe) Player.Controller2D.Collision.OnCeilingBump += OnCeilingBump;
            else Player.Controller2D.Collision.OnCeilingBump -= OnCeilingBump;
        }
        
        protected void SubscribeOnJumpPress(bool subscribe)
        {
            if(subscribe) InputManager.PlayerInput.OnJumpChange += OnJumpPress;
            else InputManager.PlayerInput.OnJumpChange -= OnJumpPress;
        }
        
        #endregion

        #region Common Functions

        protected void OnLanding()
        {
            Player.SetState(Player.States.GroundedState);
        }

        protected void OnTakeOff()
        {
            Player.SetState(Player.States.FallingState);
        }

        protected void OnCeilingBump()
         {
            Player.SetState(Player.States.FallingState);
        }
        
        protected void OnJumpPress(bool isPress)
        {
            if(isPress) Player.SetState(Player.States.JumpingState);
        }
        
        #endregion
        
    }
}