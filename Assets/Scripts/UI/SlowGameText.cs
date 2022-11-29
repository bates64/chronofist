using System.Collections;
using UnityEngine;

namespace Ui {
    [RequireComponent(typeof(GameText))]
    public class SlowGameText : MonoBehaviour {
        [TextArea] public string eventualText = "SlowGameText";
        public float delay = 0.05f;
        public float delayComma = 0.2f;
        public float delayPeriod = 0.5f;
        public bool speedUpWhenBackDown;

        public bool IsDone => gameText != null && gameText.NumberOfCharsToRender == eventualText.Length;

        private GameText gameText;
        private Coroutine co;

        private void Awake() {
            gameText = GetComponent<GameText>();
        }

        private void Start() {
            if (gameText != null)
                PrintText(eventualText);
        }

        private IEnumerator ShowText() {
            // Print each character one at a time
            gameText.Text = eventualText;
            gameText.NumberOfCharsToRender = 0;
            foreach (var c in eventualText) {
                gameText.NumberOfCharsToRender += 1;
                var multiplier = speedUpWhenBackDown && InputManager.InterfaceInput.back ? 0.1f : 1f;
                yield return new WaitForSeconds(multiplier * c switch {
                    ',' => delayComma,
                    '.' => delayPeriod,
                    '!' => delayPeriod,
                    _ => delay
                });
            }
        }

        public void PrintText(string text) {
            if (co != null) {
                StopCoroutine(co);
            }

            eventualText = text;
            co = StartCoroutine(ShowText());
        }
    }
}
