using UnityEngine;

namespace Village {
    public class HouseTop : MonoBehaviour {
        public float visibleWhenXGreaterThan;
        public GameObject triggerer;

        private float startX;
        private float endX;

        private void Start() {
            startX = transform.position.x;
            endX = startX - 40f;
        }

        private void Update() {
            var targetX = startX;

            if (triggerer.transform.position.x < visibleWhenXGreaterThan) {
                targetX = endX;
            }

            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * 4f),
                transform.position.y,
                transform.position.z
            );
        }
    }
}
