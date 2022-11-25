using System.Collections;
using UnityEngine;

namespace Ui {
    [RequireComponent(typeof(GameText))]
    public class SlowGameText : MonoBehaviour {
        public string eventualText = "SlowGameText";
        public float delay = 0.05f;
        public float delayComma = 0.2f;
        public float delayPeriod = 0.5f;

        public bool IsDone => gameText.Text == eventualText;

        private GameText gameText;

        private void Awake() {
            gameText = GetComponent<GameText>();
        }

        private void Start() {
            if (gameText != null)
                StartCoroutine(ShowText());
        }

        private IEnumerator ShowText() {
            // Print each character one at a time
            // TODO: use a new GameText.hideAfterIndex instead of resetting the text each time
            gameText.Text = "";
            foreach (var c in eventualText) {
                gameText.Text += c;
                yield return new WaitForSeconds(c switch {
                    ',' => delayComma,
                    '.' => delayPeriod,
                    '!' => delayPeriod,
                    _ => delay
                });
            }
        }
    }
}
