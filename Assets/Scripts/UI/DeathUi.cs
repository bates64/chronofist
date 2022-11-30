using UnityEngine;

namespace Ui {
    public class DeathUi : MonoBehaviour {
        public Health.Health health;
        public GameObject world;
        public GameObject deathScreen;
        public AudioSource audioToStop;

        private void Start() {
            deathScreen.SetActive(false);
        }

        private void Update() {
            if (health.isDead) {
                InputManager.SetMode(InputManager.Mode.Interface);
                //world.SetActive(false);
                deathScreen.SetActive(true);
                audioToStop.volume = Mathf.Lerp(audioToStop.volume, 0, Time.deltaTime * 4f);
            }
        }
    }
}
