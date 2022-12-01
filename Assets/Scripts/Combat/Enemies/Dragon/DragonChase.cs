using UnityEngine;

namespace Combat.Enemies.Dragon
{
    public class DragonChase : DragonState
    {
        public DragonChase(Dragon myEnemy) : base(myEnemy)
        {
        }

        public override int Id => 0;

        public override void EnterState()
        {

        }

        public override void Update()
        {
            if (MyEnemy.WithinFire)
            {
                MyEnemy.SetState(MyEnemy.FireState);
                return;
            }
            if (!MyEnemy.WithinAggro)
            {
                MyEnemy.SetState(MyEnemy.ReturnToHomeState);
                return;
            }
            Chase();
        }

        private void Chase()
        {
            Vector2 direction = Enemy.PlayerPosition - (Vector2) MyEnemy.transform.position;
            Vector2 movement = direction.normalized * (MyEnemy.Speed * MyEnemy.DeltaTime);
            MyEnemy.Controller.Move(movement);
            MyEnemy.Flip((int)Mathf.Sign(direction.x),MyEnemy.transform);
        }

        public override void ExitState()
        {

        }
    }
}
