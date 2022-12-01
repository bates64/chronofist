namespace Combat.Enemies.Goblin
{
    public class GoblinAirborneState : GoblinState
    {
        public GoblinAirborneState(Goblin myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override int Id => 3;
        public float timer;

        public override void EnterState()
        {
            timer = MyEnemy.StunTime;
            base.EnterState();
        }

        public override void Update()
        {
            timer -= MyEnemy.DeltaTime;
            MyEnemy.Gravity();
            MyEnemy.Controller.Move(MyEnemy.Velocity * MyEnemy.DeltaTime);
            if (MyEnemy.Controller.isGrounded && timer <= 0)
            {
                MyEnemy.SetState(MyEnemy.IdleState);
                MyEnemy.Home = MyEnemy.transform.position;
            }
        }

        public override void ExitState()
        {
            timer = 0;
        }
    }
}
