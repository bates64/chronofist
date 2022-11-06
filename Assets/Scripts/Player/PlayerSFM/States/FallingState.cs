using Player_.PlayerSFM.States.BaseClasses;
using UnityEngine;

namespace Player_.PlayerSFM.States
{
    public class FallingState : PlayerState
    {
        public override int StateId => 2;

        public override void EnterState(Player machine)
        {
            base.EnterState(machine);
        }

        protected override void RegisterSubscriptions()
        {
            Subscriptions.Add(SubscribeLanding);
        }

        public override void Update()
        {
            CommonUpdate();
        }

        public override string ToString()
        {
            return "Falling State";
        }
    }
}