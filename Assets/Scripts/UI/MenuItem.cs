using Effects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui {
    public class MenuItem : MonoBehaviour {
        public string transitionToSceneName;
        public InputManager.Mode setInputManagerMode;

        private Menu menu;

        public void Start() {
            menu = GetComponentInParent<Menu>();
        }

        public void Interact() {
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
