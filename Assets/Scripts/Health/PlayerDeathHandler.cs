using Effects;
using UnityEngine;
using World;

namespace Health {
    public class PlayerDeathHandler : DeathHandler {
        public override void OnDeath() {
            // See also DeathUi

            // Pause time
            TimeEffect.Spawn(1000f, 0.2f);
        }
    }
}
