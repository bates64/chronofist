using UnityEngine;

namespace Health {
    public class TimeBar : MonoBehaviour {
        public Health track;
        public Vector3 emptyPosition;
        public Vector3 fullPosition;
        public float speed = 10f;

        public void Update() {
            var targetPosition = Vector3.Lerp(emptyPosition, fullPosition, track.health / track.maxHealth);

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);
        }
    }
}
