namespace Combat.Enemies
{
    public abstract class EnemyState<T> where T : Enemy
    {
        public T MyEnemy;

        public abstract int Id { get; }

        public EnemyState(T myEnemy)
        {
            MyEnemy = myEnemy;
        }

        public abstract void EnterState();

        public abstract void Update();

        public abstract void ExitState();
    }
}
