using Effects;
using UnityEngine;

namespace Health {
    public class PlayerDeathHandler : DeathHandler {
        public override void OnDeath() {
            Debug.Log("Player died!");
        }
    }
}
