using Player_.PlayerSFM.States.BaseClasses;
using UnityEngine;

namespace Player_.PlayerSFM.States
{
    public class JumpingState : PlayerState
    {
        private float _ascendingVelocity;
        public override int StateId => 3;

        private float MinimumVelocity => 0.1f;
        
        public override void EnterState(Player machine)
        {
            base.EnterState(machine);
            _ascendingVelocity = Player.JumpVelocity;
        }

        public override void Update()
        {
            ApplyVerticalVelocities();
            ApplyMovementVelocity();
            float realVel = ApplyVerticalVelocities();
            PerformMovement();
            if (realVel <= 0) OnFall();
        }

        public float ApplyVerticalVelocities()
        {
            Player.Gravity.AddForce(Physics.LocalTime.deltaTimeAt(Player.transform.position));
            float realVelocity = _ascendingVelocity + Player.Gravity.AccumulatedVelocity;
            Player.Push(new Vector2(0,realVelocity));
            return realVelocity;
        }
        
        protected override void RegisterSubscriptions()
        {
            Subscriptions.Add(SubscribeJumpRelease);
            Subscriptions.Add(SubscribeLanding);
            Subscriptions.Add(SubscribeOnCeilingBump);
        }

        protected void SubscribeJumpRelease(bool subscribe)
        {
            if(subscribe) InputManager.PlayerInput.OnJumpChange += OnJumpRelease;
            else InputManager.PlayerInput.OnJumpChange -= OnJumpRelease;
        }

        private void OnJumpRelease(bool isPress)
        {
            if(!isPress) EndAscension();
        }

        private void EndAscension()
        {
            float realVelocity = _ascendingVelocity + Player.Gravity.AccumulatedVelocity;
            if (realVelocity > MinimumVelocity)
            {
                _ascendingVelocity = -Player.Gravity.AccumulatedVelocity + MinimumVelocity;
            }
        }
        
        protected void OnFall()
        {
            Debug.Log("Fall");
            Player.Gravity.ResetForce();
            Player.SetState(Player.States.FallingState);
        }
        
        public override string ToString()
        {
            return "Jumping State";
        }
    }
}