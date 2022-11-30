using UnityEngine;
using UnityEngine.SceneManagement;
using Effects;

namespace Village {
    public class DungeonTrigger : MonoBehaviour {
        public string sceneName;
        public float whenYLessThan;
        public GameObject triggerer;
        public float wipeTime = 1f;

        private GameObject wipeEffect;
        private float timeSinceTriggered;

        private void Update() {
            if (wipeEffect == null) {
                // Check for trigger
                if (triggerer.transform.position.y < whenYLessThan) {
                    wipeEffect = WipeEffect.Spawn(wipeTime * 2f, -1f);
                }
            } else {
                // Wait for first half of wipe to finish
                timeSinceTriggered += Time.deltaTime;
                if (timeSinceTriggered > wipeTime) {
                    SceneManager.LoadScene(sceneName);
                }
            }
        }
    }
}
