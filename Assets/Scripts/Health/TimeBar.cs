using UnityEngine;

namespace Health {
    public class TimeBar : MonoBehaviour {
        public Health track;
        public Vector2 emptyPosition;
        public Vector2 fullPosition;
        public float speed = 10f;

        public void Update() {
            var targetPosition = Vector2.Lerp(emptyPosition, fullPosition, track.health / track.maxHealth);

            transform.localPosition = Vector2.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);
        }
    }
}
