using Player_.PlayerSFM.States.BaseClasses;

namespace Player_.PlayerSFM.States
{
    public class GroundedState : PlayerState
    {
        private float _movementVelocity;
        #region Propertiees

        public override int StateId
        {
            get
            {
                if (_movementVelocity != 0) return RunningId;
                return IdleId;
            }
        }

        private int IdleId => 0;
        private int RunningId => 1;

        #endregion

        public override void EnterState(Player machine)
        {
            base.EnterState(machine);
            _movementVelocity = 0;
            Player.Gravity.ResetForce();
        }

        public override void Update()
        {
            Player.Gravity.ResetForce();
            CommonUpdate(ref _movementVelocity);
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