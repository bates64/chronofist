using Cinemachine;
using UnityEngine;
using Ui;

namespace Npc {
    public class SpeechTrigger : MonoBehaviour {
        [Header("Globals")]
        public GameObject player;
        public SpeechUi speechUi;

        [Header("Options")]
        public GameObject speechBubble;
        public CinemachineVirtualCamera virtualCamera;
        public float triggerDistance = 10f;
        [TextArea] public string[] dialogue = { "Hello, world!" };

        private bool isSpeaking;

        private void Update() {
            // Show speech bubble if player is close enough
            if (!isSpeaking && Vector3.Distance(player.transform.position, transform.position) < triggerDistance) {
                speechBubble.SetActive(true);
                CheckInteract();
            } else {
                speechBubble.SetActive(false);
                // TODO: ideally run animation backwards
            }

            // Check for end of dialogue
            if (isSpeaking && !speechUi.visible) {
                isSpeaking = false;
                virtualCamera.gameObject.SetActive(false);
                InputManager.SetMode(InputManager.Mode.Player);
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, triggerDistance);
        }

        private void CheckInteract() {
            if (InputManager.PlayerInput.Attack) {
                speechUi.dialogue = dialogue;
                speechUi.dialogueIndex = 0;
                speechUi.Trigger();

                isSpeaking = true;
                virtualCamera.gameObject.SetActive(true);
                InputManager.SetMode(InputManager.Mode.Interface);
            }
        }
    }
}
