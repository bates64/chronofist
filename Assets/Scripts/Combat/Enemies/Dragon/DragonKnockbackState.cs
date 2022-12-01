using UnityEngine;

namespace Combat.Enemies.Dragon
{
    public class DragonKnockbackState : DragonState
    {
        public DragonKnockbackState(Dragon myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override int Id => 2;
        private float _stunTimer;

        public override void EnterState()
        {
            base.EnterState();
            _stunTimer = MyEnemy.StunTime;
        }

        public override void Update()
        {
            _stunTimer -= MyEnemy.DeltaTime;
            MyEnemy.Gravity();
            MyEnemy.Controller.Move(MyEnemy.Velocity * MyEnemy.DeltaTime);
            if (_stunTimer <= 0)
            {
                MyEnemy.SetState(MyEnemy.IdleState);
            }
        }

        public override void ExitState()
        {
            _stunTimer = 0;
            MyEnemy.Velocity = Vector2.zero;
        }
    }
}
