using UnityEngine;

namespace Effects {
    public class Effect : MonoBehaviour {
        public float TimeToLive;
        protected bool isLastUpdate = false;

        public void LateUpdate() {
            TimeToLive -= Time.deltaTime; // Not using LocalTime!

            if (TimeToLive <= 0f) {
                if (isLastUpdate) {
                    Destroy(gameObject);
                } else {
                    // We get to live one more frame!
                    isLastUpdate = true;
                }
            }
        }
    }
}
