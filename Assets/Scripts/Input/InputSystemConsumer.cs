namespace Input {
    public class InputSystemConsumer {
        protected InputActionsPlayer _actions = new();

        public void Disable() {
            _actions.Disable();
        }

        public void Enable() {
            _actions.Enable();
        }
    }
}
