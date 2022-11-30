using UnityEngine;

namespace Ui {
    public class MenuCursor : MonoBehaviour {
        public Transform follow;
        public float speed = 10f;
        public Vector3 offset = new Vector3(-1.5f, 0.625f, 0f);

        private void Update() {
            if (follow == null)
                return;

            var target = follow.transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
        }
    }
}
