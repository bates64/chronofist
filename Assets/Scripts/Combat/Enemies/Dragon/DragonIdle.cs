namespace Combat.Enemies.Dragon
{
    public class DragonIdle : DragonState
    {
        public DragonIdle(Dragon myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        private float idleTime;

        public override int Id => 0;

        public override void EnterState()
        {
            base.EnterState();
            SetTimer(MyEnemy.IdleTime);
        }

        public void SetTimer(float t)
        {
            idleTime = t;
        }

        public override void Update()
        {
            idleTime -= MyEnemy.DeltaTime;
            if (idleTime <= 0)
            {
                if (MyEnemy.WithinAggro)
                {
                    MyEnemy.SetState(MyEnemy.ChaseState);
                }
                else
                {
                    MyEnemy.SetState(MyEnemy.ReturnToHomeState);
                }
            }
        }

        public override void ExitState()
        {

        }
    }
}
