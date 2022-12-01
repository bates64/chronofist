using Health;
using UnityEngine;
using Physics;

namespace World {
    /// <summary>
    ///     Gives the player control of the actual player object when he first touches the floor.
    /// </summary>
    public class PlayerControlGiver : MonoBehaviour {
        public Player player;
        public HealthTimeDepletion healthTimeDepletion;
        public GameObject objectToAwake;

        public void Awake() {
            // Disable input
            InputManager.SetMode(InputManager.Mode.None);
        }

        public void Update() {
            if (player.HasTouchedFloor) {
                // Enable input
                InputManager.SetMode(InputManager.Mode.Player);

                // Side effects
                if (healthTimeDepletion != null)
                    healthTimeDepletion.depletionRate = 1f;
                if (objectToAwake != null)
                    objectToAwake.SetActive(true);

                // We're done
                Destroy(this);
            }
        }
    }
}
