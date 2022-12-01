using Cinemachine;
using Physics;
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

        [Header("Speaker")]
        public Sprite characterNameSprite;
        public Sprite characterPortraitSprite;

        private bool isSpeaking;

        private void Awake() {
            if (player == null) {
                player = FindObjectOfType<Player>().gameObject;
            }

            if (speechUi == null) {
                speechUi = FindObjectOfType<SpeechUi>();
            }
        }

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
                if (virtualCamera != null) virtualCamera.gameObject.SetActive(false);
                InputManager.SetMode(InputManager.Mode.Player);
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, triggerDistance);
        }

        private void CheckInteract() {
            if (InputManager.PlayerInput.Attack) {
                TriggerSpeech();
            }
        }

        public void TriggerSpeech() {
            speechUi.dialogue = dialogue;
            speechUi.dialogueIndex = 0;
            speechUi.SetCharacterSprites(characterNameSprite, characterPortraitSprite);
            speechUi.Trigger();

            isSpeaking = true;
            if (virtualCamera != null) virtualCamera.gameObject.SetActive(true);
            InputManager.SetMode(InputManager.Mode.Interface);
        }
    }
}
