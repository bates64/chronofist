using UnityEngine;

namespace UI {
    public class IntroScreen : MonoBehaviour {
        public GameObject nextScreen;
        public float lifetime = 5f;

        private float _timeAlive;

        private void Update() {
            _timeAlive += Time.deltaTime;

            if (_timeAlive >= lifetime) {
                if (nextScreen != null) {
                    nextScreen.SetActive(true);
                }

                gameObject.SetActive(false);
            }
        }
    }
}
