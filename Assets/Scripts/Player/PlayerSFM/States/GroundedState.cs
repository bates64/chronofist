using Player_.PlayerSFM.States.BaseClasses;

namespace Player_.PlayerSFM.States {
    public class GroundedState : PlayerState {
        #region Propertiees

        public override int StateId {
            get {
                if (Player.MovementVelocity.SqrMagnitude() > 0.0f) return RunningId;
                return IdleId;
            }
        }

        private int IdleId => 0;
        private int RunningId => 1;

        #endregion

        public override void EnterState(Player machine)
        {
            base.EnterState(machine);
            Player.Gravity.ResetForce();
        }

        public override void Update()
        {
            Player.Gravity.ResetForce();
            CommonUpdate();
        }
        
        protected override void RegisterSubscriptions()
        {
            Subscriptions.Add(SubscribeOnTakeOff);
            Subscriptions.Add(SubscribeOnJumpPress);
        }
        
        public override string ToString()
        {
            return "Grounded State";
        }
    }
}