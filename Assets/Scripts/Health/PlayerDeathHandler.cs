using Effects;
using UnityEngine;

namespace Health {
    public class PlayerDeathHandler : DeathHandler {
        public override void OnDeath() {
            Debug.Log("Player died!");
            InputManager.PlayerInput.Disable();
            TimeEffect.Spawn(0.2f, 1f / 12f);

            // TODO: some other stuff
        }
    }
}
