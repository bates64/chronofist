using UnityEngine;

namespace Ui {
    public class BobUpDown : MonoBehaviour {
        public float amplitude;
        public float period;

        private void FixedUpdate() {
            var dy = Mathf.Sin(Time.time * period) * amplitude;
            transform.position += new Vector3(0, dy, 0);
        }
    }
}
