using UnityEngine;

namespace Combat.Enemies.Goblin
{
    public class GoblinWalk : GoblinState
    {
        public GoblinWalk(Goblin myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override int Id => 1;
        protected Vector2 target;

        protected virtual float speed => MyEnemy.walkSpeed;
        public override void EnterState()
        {
            base.EnterState();
            MyEnemy.Velocity = Vector2.zero;
            MyEnemy.Velocity.y = -0.1f;
            PickTarget();
        }

        protected virtual void PickTarget()
        {
            target = MyEnemy.Home;
            target.x += Random.Range(-MyEnemy.WanderRadius, MyEnemy.WanderRadius);
            target.y += Random.Range(-MyEnemy.WanderRadius, MyEnemy.WanderRadius);
        }

        public override void Update()
        {
            if (MyEnemy.WithinAggro)
            {
                MyEnemy.SetState(MyEnemy.ChargeState);
                return;
            }
            Walk();
        }

        protected void Walk()
        {
            float dif = target.x - MyEnemy.transform.position.x;
            int dir = (int) Mathf.Sign(dif);
            float vel = dir * speed * MyEnemy.DeltaTime;
            if (Mathf.Abs(vel) > Mathf.Abs(dif))
            {
                MyEnemy.Controller.Move(new Vector2(dif,-0.1f));
                MyEnemy.SetState(MyEnemy.IdleState);
            }
            else
            {
                MyEnemy.Controller.Move(new Vector2(vel,-0.1f));
            }
            MyEnemy.Flip(dir,MyEnemy.transform);
        }

        public override void ExitState()
        {

        }
    }
}
