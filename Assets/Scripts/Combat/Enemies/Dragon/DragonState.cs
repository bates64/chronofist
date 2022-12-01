namespace Combat.Enemies.Dragon
{
    public abstract class DragonState : EnemyState<Dragon>
    {
        public DragonState(Dragon myEnemy) : base(myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public override void EnterState()
        {
            MyEnemy.Animator.SetInteger(Enemy.StateId,Id);
            MyEnemy.Animator.SetTrigger(Enemy.Play);
            MyEnemy.Animator.Update(0);
        }

    }
}
