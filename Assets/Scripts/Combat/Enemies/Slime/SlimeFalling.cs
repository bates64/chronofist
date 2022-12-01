namespace Combat.Enemies.Slime
{
    public class SlimeFalling : SlimeState
    {
        public override int Id => 1;

        public SlimeFalling(Slime myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override void EnterState()
        {
            base.EnterState();
            MyEnemy.Controller.collision.OnLanding += OnLanding;
        }

        public override void Update()
        {
            MyEnemy.Gravity();
            MyEnemy.Controller.Move(MyEnemy.Velocity * MyEnemy.DeltaTime);
        }

        public override void ExitState()
        {
            MyEnemy.Controller.collision.OnLanding -= OnLanding;
        }

        private void OnLanding()
        {
            MyEnemy.SetState(MyEnemy.IdleState);
        }


    }
}
