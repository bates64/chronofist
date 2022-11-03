namespace General
{
    public abstract class FsmState<T>
    {
        private T _machine;
        public T Machine => _machine;
        
        public virtual void EnterState(T machine)
        {
            _machine = machine;
        }

        public abstract void Update();

        public virtual void ExitState()
        {
            _machine = default;
        }
    }
}