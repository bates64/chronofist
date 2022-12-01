using General;
using UnityEngine;

namespace Effects {
    public class WipeIntoScene : MonoBehaviour {
        public float speed = 2f;

        private void Update() {
            transform.Translate(Vector3.up * Util.PIXEL * Time.deltaTime * speed);

            if (transform.position.y > 40f) {
                InputManager.SetMode(InputManager.Mode.Player);
                Destroy(gameObject);
            }
        }
    }
}
