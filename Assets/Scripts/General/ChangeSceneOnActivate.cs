using UnityEngine;
using UnityEngine.SceneManagement;

namespace General {
    public class ChangeSceneOnActivate : MonoBehaviour {
        public string sceneName;

        private void OnEnable() {
            if (string.IsNullOrEmpty(sceneName)) {
                Debug.LogError("Scene name is not set");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
