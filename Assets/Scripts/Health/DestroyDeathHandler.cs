namespace Health {
    /// <summary>
    ///     A component that destroys the GameObject when it dies, with no other effects.
    /// </summary>
    public class DestroyDeathHandler : DeathHandler {
        public override void OnDeath() {
            Destroy(gameObject);
        }
    }
}
