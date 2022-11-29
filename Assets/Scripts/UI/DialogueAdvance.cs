using UnityEngine;

namespace Ui {
    [ExecuteAlways]
    public class DialogueAdvance : MonoBehaviour {
        public bool visible;

        private void Update() {
            var targetY = visible ? -4.4f : 0f;
            var y = Application.isPlaying
                ? Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * 10f)
                : targetY;

            transform.localPosition = new Vector3(
                transform.localPosition.x,
                y,
                transform.localPosition.z
            );
        }
    }
}
