using Effects;
using UnityEngine;

namespace Ui {
    public class IntroScreen : MonoBehaviour {
        public GameObject nextScreen;
        public float lifetime = 5f;

        private float _timeAlive;
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
            _timeAlive += Time.deltaTime;

            if (_timeAlive >= lifetime) {
                if (didWipeEffect) {
                    if (_timeAlive >= (lifetime + WipeEffectTime / 2f)) {
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
