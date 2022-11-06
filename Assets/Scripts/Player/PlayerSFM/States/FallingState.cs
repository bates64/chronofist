using Player_.PlayerSFM.States.BaseClasses;
using UnityEngine;

namespace Player_.PlayerSFM.States
{
    public class FallingState : PlayerState
    {
        private float _movementVelocity;
        public override int StateId => 2;

        public override void EnterState(Player machine)
        {
            base.EnterState(machine);
            _movementVelocity = 0;
        }

        protected override void RegisterSubscriptions()
        {
            Subscriptions.Add(SubscribeLanding);
        }

        public override void Update()
        {
            CommonUpdate(ref _movementVelocity);
        }

        public override string ToString()
        {
            return "Falling State";
        }
    }
}