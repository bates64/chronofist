using System.Collections.Generic;
using General;
using UnityEngine;

namespace Player_.PlayerSFM.States.BaseClasses
{
    public abstract class PlayerState : FsmState<Player>
    {
        protected Player Player => Machine;
        protected readonly List<Util.DBool> Subscriptions = new List<Util.DBool>();
        
        public virtual bool PerformMovement => true;
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
            float realVel = Player.Gravity.AccumulatedVelocity + Player.Velocity.y;
            float multiplier = realVel <= 0 ? 2 : 1;
            Player.AddVelocity(new Vector2(0,realVel * multiplier));
        }

        protected void ApplyAscendingVelocity(float ascendingVelocity)
        {
            Player.AddVelocity(new Vector2(0,ascendingVelocity));
        }
        
        protected void ApplyMovementVelocity(ref float storedVelocity)
        {
            storedVelocity = InputManager.PlayerInput.Movement.x * Player.MovementSpeed;
            Player.AddVelocity(new Vector2(storedVelocity,0));
        }

        protected void CommonUpdate(ref float storedVelocity)
        {
            ApplyGravity();
            ApplyMovementVelocity(ref storedVelocity);
        }
        
        #endregion

        #region Common Subscription Functions
        
        protected void SubscribeLanding(bool subscribe)
        {
            if(subscribe) Player.Controller2D.Collisions.OnLanding += OnLanding;
            else Player.Controller2D.Collisions.OnLanding -= OnLanding;
        }

        protected void SubscribeOnTakeOff(bool subscribe)
        {
            if(subscribe) Player.Controller2D.Collisions.OnTakeOff += OnTakeOff;
            else Player.Controller2D.Collisions.OnTakeOff -= OnTakeOff;
        }

        protected void SubscribeOnCeilingBump(bool subscribe)
        {
            if(subscribe) Player.Controller2D.Collisions.OnBonk += OnCeilingBump;
            else Player.Controller2D.Collisions.OnBonk -= OnCeilingBump;
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
            Debug.Log("BUMP");     
            Player.SetState(Player.States.FallingState);
        }
        
        protected void OnJumpPress(bool isPress)
        {
            if(isPress) Player.SetState(Player.States.JumpingState);
        }
        
        #endregion
        
    }
}