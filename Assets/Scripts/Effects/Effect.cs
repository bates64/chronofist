using UnityEngine;

namespace Effects {
    public abstract class Effect : MonoBehaviour {
        public float TimeToLive;
        protected bool isLastUpdate;

        public void LateUpdate() {
            TimeToLive -= Time.deltaTime; // Not using LocalTime!

            if (TimeToLive <= 0f) {
                if (isLastUpdate) {
                    Destroy(gameObject);
                } else

                    // We get to live one more frame!
                {
                    isLastUpdate = true;
                }
            }
        }
    }
}
