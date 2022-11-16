using General;
using UnityEngine;

namespace Pause {
    public class PauseManager : Singleton<PauseManager> {
        public bool isPaused { get; private set; }

        private void Update() {
            if (isPaused)
                if (InputManager.InterfaceInput.back)
                    Unpause();
        }

        public void OnGUI() {
            if (isPaused) GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "Paused n shit");
        }

        protected override void init() {
            InputManager.PlayerInput.OnPause += Pause;
        }

        public void Pause() {
            Debug.Log("Pause");
            isPaused = true;
            InputManager.SetMode(InputManager.Mode.Interface);
        }

        public void Unpause() {
            Debug.Log("Unpause");
            isPaused = false;
            InputManager.SetMode(InputManager.Mode.Player);
        }
    }
}
