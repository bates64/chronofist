namespace Combat.Enemies.Goblin
{
    public abstract class GoblinState : EnemyState<Goblin>
    {
        public GoblinState(Goblin myEnemy) : base(myEnemy)
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
