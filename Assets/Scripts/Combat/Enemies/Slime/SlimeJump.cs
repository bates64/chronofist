using Physics;
using UnityEngine;

namespace Combat.Enemies.Slime
{
    public class SlimeJump : SlimeState
    {
        public override int Id => 1;
        public Vector2 JumpForce;

        public SlimeJump(Slime myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public bool PlayerInRange => Mathf.Abs(Vector2.Distance(MyEnemy.transform.position,Enemy.PlayerPosition)) <= MyEnemy.Radius;

        public int PlayerDirection => MyEnemy.transform.position.x > Enemy.PlayerPosition.x ? -1 : 1;

        public override void EnterState()
        {
            base.EnterState();
            MyEnemy.Velocity = JumpForce;
            MyEnemy.Controller.collision.OnLanding += OnLanding;
        }

        public override void Update()
        {
            MyEnemy.Gravity();
            MyEnemy.Controller.Move(MyEnemy.Velocity * MyEnemy.DeltaTime);
        }

        public override void ExitState()
        {
            MyEnemy.Controller.collision.OnLanding += OnLanding;
        }

        public void StartJump()
        {
            int dir = 0;
            if (PlayerInRange)
            {
                dir = (int) Mathf.Sign(PlayerDirection);
                JumpForce = MyEnemy.BigJumpForce;
                JumpForce.x *= Mathf.Sign(dir);
            }
            else
            {
                dir = (int) Mathf.Sign(Random.Range(-1, 1));
                JumpForce = MyEnemy.SmallJumpForce;
                JumpForce.x *= Mathf.Sign(dir);
            }
            MyEnemy.Flip(dir,MyEnemy.transform);
            MyEnemy.SetState(this);
        }



        private void OnLanding()
        {
            MyEnemy.SetState(MyEnemy.IdleState);
        }

    }
}
