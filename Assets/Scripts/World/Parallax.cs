using UnityEngine;

namespace World {
    [ExecuteAlways]
    public class Parallax : MonoBehaviour {
        public Vector3 multiplier = new Vector3(1f, 0f, 0f);

        private void Update() {
            var cam = Camera.current;
            if (cam == null) return;
            var target = cam.transform.position;
            transform.localPosition = Vector3.Scale(target, multiplier);
        }
    }
}
