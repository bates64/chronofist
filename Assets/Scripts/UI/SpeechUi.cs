using UnityEngine;

namespace Ui {
    [ExecuteAlways]
    public class SpeechUi : MonoBehaviour {
        public bool visible;
        public SlowGameText text;
        public DialogueAdvance advance;
        public string[] dialogue;
        public int dialogueIndex;

        private void Update() {
            MoveBox();
            advance.visible = text.IsDone;

            if (visible) {
                if (advance.visible) {
                    if (InputManager.InterfaceInput.interact) {
                        Next();
                    }
                }
            }
        }

        private void MoveBox() {
            var targetY = visible ? 0 : -36f;
            var y = Application.isPlaying
                ? Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * 10f)
                : targetY;

            transform.localPosition = new Vector3(
                transform.localPosition.x,
                y,
                transform.localPosition.z
            );
        }

        public void Trigger() {
            dialogueIndex--;
            Next();
        }

        private void Next() {
            dialogueIndex++;

            if (dialogueIndex >= dialogue.Length) {
                visible = false;
            } else {
                text.PrintText(dialogue[dialogueIndex]);
                visible = true;
            }
        }
    }
}
