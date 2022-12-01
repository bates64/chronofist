using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui {
    public class DeathUi : MonoBehaviour {
        public Health.Health health;
        public GameObject world;
        public GameObject deathScreen;
        public AudioSource audioToStop;

        private bool triggered;

        private void Start() {
            deathScreen.SetActive(false);
        }

        private void Update() {
            if (health.isDead) {
                InputManager.SetMode(InputManager.Mode.Interface);
                //world.SetActive(false);
                deathScreen.SetActive(true);
                audioToStop.volume = Mathf.Lerp(audioToStop.volume, 0, Time.deltaTime * 4f);

                // hotfix due to death menu not working
                if (!triggered) {
                    triggered = true;
                    Invoke(nameof(BackToVillage), 1f);
                }
            }
        }

        private void BackToVillage() {
            SceneManager.LoadScene("VillageScene");
        }
    }
}
