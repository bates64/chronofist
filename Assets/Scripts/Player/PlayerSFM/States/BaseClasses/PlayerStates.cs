namespace Player_.PlayerSFM.States.BaseClasses
{
    public class PlayerStates
    {
        public readonly FallingState FallingState = new FallingState();
        public readonly GroundedState GroundedState = new GroundedState();
        public readonly JumpingState JumpingState = new JumpingState();

        public void Init()
        {
            FallingState.Init();
            GroundedState.Init();
            JumpingState.Init();
        }
    }
}