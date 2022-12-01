using UnityEngine;

namespace Combat.Enemies.Dragon
{
    public class DragonFireState : DragonState
    {
        public DragonFireState(Dragon myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override int Id => 1;

        public override void EnterState()
        {
            base.EnterState();
            Vector2 direction = Enemy.PlayerPosition - (Vector2) MyEnemy.transform.position;
            MyEnemy.Flip((int)Mathf.Sign(direction.x),MyEnemy.transform);
        }

        public override void Update()
        {
            if (MyEnemy.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                End();
            }
        }

        private void End()
        {
            MyEnemy.SetState(MyEnemy.IdleState);
        }

        public override void ExitState()
        {

        }
    }
}
