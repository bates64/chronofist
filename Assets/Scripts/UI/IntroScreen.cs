using Effects;
using UnityEngine;

namespace Ui {
    public class IntroScreen : MonoBehaviour {
        public GameObject nextScreen;
        public float delayUntilNextScreen = 1f;

        private float timeSinceDone;
        private bool didWipeEffect;
        private SlowGameText text;

        private const float WipeEffectTime = 1f;

        private void Awake() {
            // Find SlowGameText component in child, and disable it for a bit
            text = GetComponentInChildren<SlowGameText>();
            if (text) {
                text.gameObject.SetActive(false);
                Invoke(nameof(EnableText), WipeEffectTime / 2f);
            }
        }

        private void EnableText() {
            text.gameObject.SetActive(true);
        }

        private void Update() {
            if (text.IsDone)
                timeSinceDone += Time.deltaTime;

            if (timeSinceDone > delayUntilNextScreen) {
                if (didWipeEffect) {
                    if (timeSinceDone >= (delayUntilNextScreen + WipeEffectTime / 2f)) {
                        if (nextScreen != null) {
                            nextScreen.SetActive(true);
                        }

                        gameObject.SetActive(false);
                    }
                } else {
                    WipeEffect.Spawn(WipeEffectTime);
                    didWipeEffect = true;
                }
            }
        }
    }
}
