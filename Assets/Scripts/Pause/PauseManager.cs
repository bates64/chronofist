using General;
using UnityEngine;

namespace Pause {
    [RequireComponent(typeof(AudioSource))]
    public class PauseManager : Singleton<PauseManager> {
        public AudioClip pauseSound;
        public AudioClip unpauseSound;

        private AudioSource audioSource;

        public bool isPaused { get; private set; }

        private void Start() {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update() {
            if (isPaused) {
                if (InputManager.InterfaceInput.back) {
                    Unpause();
                }
            }
        }

        protected override void init() {
            InputManager.PlayerInput.OnPause += Pause;
        }

        public void Pause() {
            Debug.Log("Pause");
            isPaused = true;
            InputManager.SetMode(InputManager.Mode.Interface);
            if (pauseSound != null) audioSource.PlayOneShot(pauseSound);
        }

        public void Unpause() {
            Debug.Log("Unpause");
            isPaused = false;
            InputManager.SetMode(InputManager.Mode.Player);
            if (unpauseSound != null) audioSource.PlayOneShot(unpauseSound);
        }
    }
}
