using UnityEngine;

namespace Combat.Enemies.Slime
{
    public class SlimeIdle : SlimeState
    {
        public override int Id => 0;
        public float IdleTime;

        public override void EnterState()
        {
            base.EnterState();
            IdleTime = MyEnemy.IdleTime;
            MyEnemy.Velocity = Vector2.zero;
        }

        public SlimeIdle(Slime myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override void Update()
        {
            IdleTime -= MyEnemy.DeltaTime;
            if (!MyEnemy.Controller.isGrounded)
            {
                MyEnemy.SetState(MyEnemy.FallingState);
                return;
            }
            if (IdleTime <= 0)
            {
                MyEnemy.JumpState.StartJump();
                return;
            }
            MyEnemy.Velocity.y = -0.1f;
            MyEnemy.Controller.Move(MyEnemy.Velocity * MyEnemy.DeltaTime);
        }

        public override void ExitState()
        {

        }
    }
}
