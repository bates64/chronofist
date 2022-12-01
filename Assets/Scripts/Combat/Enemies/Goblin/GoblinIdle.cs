using UnityEngine;

namespace Combat.Enemies.Goblin
{
    public class GoblinIdle : GoblinState
    {
        public GoblinIdle(Goblin myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override int Id => 0;
        private float timer;
        public override void EnterState()
        {
            base.EnterState();
            MyEnemy.Velocity = Vector2.zero;
            MyEnemy.Velocity.y = -0.1f;
            timer = MyEnemy.IdleTime;
        }

        public override void Update()
        {
            timer -= MyEnemy.DeltaTime;
            MyEnemy.Controller.Move(MyEnemy.Velocity * MyEnemy.DeltaTime);
            if (timer <= 0)
            {
                if (MyEnemy.WithinAggro)
                {
                    MyEnemy.SetState(MyEnemy.ChargeState);
                }
                else
                {
                    MyEnemy.SetState(MyEnemy.WalkState);
                }
            }
        }

        public override void ExitState()
        {
            timer = 0;
        }

    }
}
