using System;
using Effects;
using Physics;
using Unity.Mathematics;
using UnityEngine;

namespace Combat.Enemies.Goblin
{
    public class Goblin : Enemy
    {
        private GoblinState CurrentState;
        private ParticleSystem _particles;
        public Animator Animator;
        public Vector2 Velocity;
        public float StunTime;
        public Vector2 Home;
        public float WanderRadius;
        public float AggroRadius;
        public float IdleTime;
        [SerializeField] private GameObject explosion;

        public GoblinIdle IdleState;
        public GoblinAirborneState AirState;
        public GoblinWalk WalkState;
        public GoblinCharge ChargeState;

        private bool WithinRange(float radius)
        {
            return Mathf.Abs(Vector2.Distance(Enemy.PlayerPosition,transform.position)) <= radius;
        }

        public bool WithinAggro => WithinRange(AggroRadius);

        protected override void Awake()
        {
            base.Awake();
            Animator = GetComponent<Animator>();
            _particles = GetComponentInChildren<ParticleSystem>();
            InitializeStates();
            health.OnDeath += Death;
            SetState(IdleState);
        }

        private void Start()
        {
            Home = transform.position;
        }

        private void InitializeStates()
        {
            IdleState = new GoblinIdle(this);
            AirState = new GoblinAirborneState(this);
            WalkState = new GoblinWalk(this);
            ChargeState = new GoblinCharge(this);
        }

        public void SetState(GoblinState nextState)
        {
            CurrentState?.ExitState();
            CurrentState = nextState;
            CurrentState?.EnterState();
        }

        protected override void Update()
        {
            Cooldowns(DeltaTime);
            CurrentState.Update();
            if (!Controller.isGrounded)
            {
                SetState(AirState);
            }
        }

        public void Gravity()
        {
            Velocity.y -= gravity * DeltaTime;
        }

        private void Death()
        {
            RestoreTime(Player.Instance);
            _particles.transform.parent = null;
            DefaultEffect effect = _particles.gameObject.AddComponent<DefaultEffect>();
            effect.TimeToLive = 1;
            Instantiate(explosion,transform.position,quaternion.identity);
            Destroy(gameObject);
        }

        protected override void WhenDamage()
        {
            _particles.Play();
            Animator.SetTrigger(DamageS);
        }

        protected override void WhenKnockBackApplied(Vector2 knockback)
        {
            Velocity = knockback;
            SetState(AirState);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Home, WanderRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AggroRadius);
        }

    }
}
