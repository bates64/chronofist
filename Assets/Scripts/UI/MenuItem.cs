using Effects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui {
    public class MenuItem : MonoBehaviour {
        public string transitionToSceneName;
        public InputManager.Mode setInputManagerMode;
        public AudioClip interactSound;
        public AudioClip selectSound;

        private Menu menu;
        private AudioSource audioSource;

        public void Start() {
            menu = GetComponentInParent<Menu>();
            audioSource = GetComponentInParent<AudioSource>();
        }

        public void Select() {
            if (audioSource != null && selectSound != null) {
                audioSource.PlayOneShot(selectSound);
            }
        }

        public void Interact() {
            if (audioSource != null && interactSound != null) {
                audioSource.PlayOneShot(interactSound);
            }

            if (!string.IsNullOrEmpty(transitionToSceneName)) {
                WipeEffect.Spawn(3f);
                Invoke(nameof(LoadScene), 1.5f);
            }

            if (setInputManagerMode != InputManager.Mode.None) {
                InputManager.SetMode(setInputManagerMode);
            }
        }

        public void LoadScene() {
            SceneManager.LoadScene(transitionToSceneName);
        }

        public void OnMouseEnter() {
            menu.SelectItem(this);
        }
    }
}
