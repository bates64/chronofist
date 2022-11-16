namespace General {
    public abstract class FsmState<T> {
        public T Machine { get; private set; }

        public virtual void EnterState(T machine) {
            Machine = machine;
        }

        public abstract void Update();

        public virtual void ExitState() {
            Machine = default;
        }
    }
}
