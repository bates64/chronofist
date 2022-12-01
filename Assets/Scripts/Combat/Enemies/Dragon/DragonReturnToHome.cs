using UnityEngine;

namespace Combat.Enemies.Dragon
{
    public class DragonReturnToHome : DragonState
    {
        public DragonReturnToHome(Dragon myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override int Id => 0;
        public Vector2 target;
        public override void EnterState()
        {
            target = MyEnemy.Home;
            target.x += Random.Range(-MyEnemy.FlyRadius, MyEnemy.FlyRadius);
            target.y += Random.Range(-MyEnemy.FlyRadius, MyEnemy.FlyRadius);
        }

        public override void Update()
        {
            if (MyEnemy.WithinAggro)
            {
                MyEnemy.SetState(MyEnemy.ChaseState);
                return;
            }
            Vector2 direction = target - (Vector2) MyEnemy.transform.position;
            Vector2 movement = direction.normalized * (MyEnemy.Speed * MyEnemy.DeltaTime);
            if (movement.magnitude >= direction.magnitude)
            {
                MyEnemy.Controller.Move(direction);
                MyEnemy.SkipToIdle();
            }
            else
            {
                MyEnemy.Controller.Move(movement);
            }
            MyEnemy.Flip((int)Mathf.Sign(direction.x),MyEnemy.transform);
        }

        public override void ExitState()
        {

        }
    }
}
