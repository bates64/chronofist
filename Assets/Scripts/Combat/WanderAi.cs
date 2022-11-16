using Physics;
using UnityEngine;

namespace Combat {
    /// <summary>
    ///    A component that implements a two-state enemy AI:
    ///     1. Wander
    ///     2. Chase
    ///
    ///    In the Wander state, the enemy will move in a random direction for a random amount of time.
    ///    If he hits a wall or goes further than his wander distance from his home, he will turn around and move in the opposite direction.
    ///
    ///    In the Chase state, the enemy will move towards the player.
    ///    If the player leaves line-of-sight or is too far away, the enemy will return to the Wander state.
    ///    TODO: If the enemy is within attack range, he will attack the player.
    ///
    ///    TODO: In either state, if the enemy can jump, he will attempt to jump over walls he is running towards.
    ///
    ///    If the enemy somehow ends up outside of his chase distance, his home will be moved.
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    public class WanderAi : MonoBehaviour {
        private enum State {
            Wander,
            Chase,
        }

        public float chaseRadius;
        public float wanderRadius = 4f;
        public float minWaitTime = 1f;
        public float maxWaitTime = 3f;
        public float minWalkTime = 1f;
        public float maxWalkTime = 3f;

        private Enemy enemy;
        private State state = State.Wander;
        private float walkTime;
        private float waitTime;

        private void Awake() {
            enemy = GetComponent<Enemy>();
        }

        private void Update() {
            // Move home if we're grounded and far away from it
            if (enemy.IsGrounded() && enemy.DistanceFromHome() > wanderRadius) {
                enemy.HomePosition = transform.position;
            }

            switch (state) {
                case State.Wander:
                    UpdateWander();
                    break;
                case State.Chase:
                    UpdateChase();
                    break;
            }
        }

        private void UpdateWander() {
            if (chaseRadius > 0f && CanSeePlayer()) {
                state = State.Chase;
                walkTime = 0f;
                waitTime = 0f;
                return;
            }

            var deltaTime = LocalTime.DeltaTimeAt(this);

            waitTime -= deltaTime;

            if (waitTime <= 0f) {
                walkTime -= deltaTime;
                if (walkTime > 0f) {
                    enemy.Walk();

                    // Turn around if we go too far from home
                    if (enemy.DistanceFromHome() > wanderRadius) {
                        var directionOfHome = Mathf.Sign(enemy.HomePosition.x - transform.position.x);
                        enemy.Walk(directionOfHome);
                    }
                } else {
                    waitTime = Random.Range(minWaitTime, maxWaitTime);
                    walkTime = Random.Range(minWalkTime, maxWalkTime);
                }
            }
        }

        private void UpdateChase() {
            if (!CanSeePlayer()) {
                state = State.Wander;
                return;
            }

            var directionOfPlayer = Mathf.Sign(Player.Instance.transform.position.x - transform.position.x);
            enemy.Walk(directionOfPlayer);
        }

        private bool CanSeePlayer() {
            var player = Player.Instance;

            // Simple distance check
            if (Vector2.Distance(transform.position, player.transform.position) >= chaseRadius) {
                return false;
            }

            // TODO: fix line-of-sight check
            /*
            // Raycast to player
            var eyePosition = transform.position + Vector3.up * -1f;
            var hit = Physics2D.Linecast(eyePosition, player.transform.position, LayerMask.NameToLayer("Level"));
            Debug.Log(hit.point);
            */

            return true;
        }

        private void OnDrawGizmos() {
            var home = Application.isPlaying ? enemy.HomePosition : transform.position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(home, wanderRadius);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}
