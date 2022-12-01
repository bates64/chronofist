namespace Combat.Enemies.Slime
{
    public abstract class SlimeState : EnemyState<Slime>
    {
        protected SlimeState(Slime myEnemy) : base(myEnemy)
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
