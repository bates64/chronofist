using UnityEngine;

namespace Ui {
    public class FlashingVisible : MonoBehaviour {
        public float period = 0.5f;
        public GameObject objectToFlash;

        private void Update() {
            objectToFlash.SetActive(Mathf.Sin(Time.time * period) > 0);
        }
    }
}
