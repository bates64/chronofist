using General;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input {
    public class InputSystemConsumer {
        protected InputActionsPlayer _actions = new InputActionsPlayer();
        
        public void Disable() {
            _actions.Disable();
        }

        public void Enable() {
            _actions.Enable();
        }
    }
}
