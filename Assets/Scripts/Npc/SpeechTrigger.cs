using System;
using UnityEngine;

namespace Npc {
    public class SpeechTrigger : MonoBehaviour {
        public GameObject player;
        public GameObject speechBubble;
        public float triggerDistance = 10f;

        private void Update() {
            if (Vector3.Distance(player.transform.position, transform.position) < triggerDistance) {
                speechBubble.SetActive(true);
                CheckInteract();
            } else {
                speechBubble.SetActive(false);
                // TODO: Reset animator
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, triggerDistance);
        }

        private void CheckInteract() {
            if (InputManager.PlayerInput.Attack) {
                // TODO: open dialogue box etc.
                InputManager.SetMode(InputManager.Mode.Interface);
            }
        }
    }
}
