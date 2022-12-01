using UnityEngine;

namespace Combat.Enemies.Goblin
{
    public class GoblinCharge : GoblinWalk
    {
        public GoblinCharge(Goblin myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override int Id => 2;

        protected override float speed => MyEnemy.walkSpeed * 2;
        private float timer;

        public override void EnterState()
        {
            timer = 3;
            base.EnterState();
        }

        protected override void PickTarget()
        {
            target = Enemy.PlayerPosition;
        }

        public override void Update()
        {
            timer -= Time.deltaTime;
            if (MyEnemy.WithinAggro)
            {
                target = Enemy.PlayerPosition;
            }
            Walk();
            if (timer <= 0)
            {
                MyEnemy.SetState(MyEnemy.IdleState);
            }
        }
    }
}
